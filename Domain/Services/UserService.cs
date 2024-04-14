using AutoMapper;
using Domain.Interfaces.Repositories;
using Domain.Interfaces.Services;
using Domain.Models;
using Domain.Objects.Requests.User;
using Domain.Objects.Responses.Asset;
using Domain.Objects.Responses.Machine;
using Domain.Utils.Constants;
using Domain.Utils.Helpers;
using Domain.Utils.Languages;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

namespace Domain.Services
{
    public class UserService(IAuthService authService, IEncryptionService encryptionService, ITotvsExternal totvsExternal, IMailerService mailerService, IUserRepository userRepository, IMapper mapper, IHttpContextAccessor contextAccessor) : IUserService
    {
        public async Task<IEnumerable<UserResultsResponse>?> Get(int currentPage, string? userName) => await userRepository.GetUserResults(currentPage, userName);

        public async Task Save(UserRequest userRequest)
        {
            var decryptedKey = DecryptKey(userRequest.UserKey);

            var document = decryptedKey.Split("-")[2];

            await userRepository.HasUserWithTheSameInfo(userRequest, document);

            var user = mapper.Map<User>(userRequest);

            SetUserPassword(user.Password, ref user);

            user.Document = document;
            user.TotvsUserId = await totvsExternal.GetCustomerId(document, document.Length == 11);

            await userRepository.Save(user);
        }

        public async Task<LogInResponse> LogIn(LogInRequest logInRequest)
        {
            var user = await userRepository.GetUserByEmail(logInRequest.Email)
                ?? throw new InvalidOperationException("InvalidUsernameOrPassword");

            var keyBytes = Encoding.ASCII.GetBytes(user.Name);
            var fullPasswordBytes = Encoding.ASCII.GetBytes(logInRequest.Password + user.Salt);
            var encryptedFullPassword = encryptionService.EncryptDeterministic(keyBytes, fullPasswordBytes);

            if (user.Password != encryptedFullPassword)
                throw new InvalidOperationException("InvalidUsernameOrPassword");

            if (await userRepository.IsFirstLogInOfDay(user.UserId) && !await totvsExternal.IsCustomerActive(user.TotvsUserId))
                throw new InvalidOperationException("InvalidLicenseKey");

            LogOut();

            var claims = new List<Claim>
            {
                new(Token.ClaimPassword, Pwd.Auth)
            };

            DateTime? expirationDate = null;
            if (logInRequest.RememberMe)
                expirationDate = DateTime.UtcNow.AddDays(30);

            var userToken = authService.GenerateToken(user.UserId, true, claims, expirationDate);
            if (userToken.IsNullOrEmpty())
                throw new InvalidOperationException("ErrorLoggingIn");

            var sessionToken = authService.GenerateToken(-1, false, claims, expirationDate);

            user.LastLogin = DateTime.Now;
            await userRepository.Update(user);

            return new LogInResponse(user.LastLogin, sessionToken);
        }

        public LogInResponse IntraLogIn(IntraLogInRequest intraLogInRequest)
        {
            if (intraLogInRequest.UserName != "adm" || intraLogInRequest.Password != Pwd.Intra)
                throw new InvalidOperationException("InvalidUsernameOrPassword");

            var claims = new List<Claim>
            {
                new(Token.ClaimPassword, Pwd.Auth)
            };

            authService.GenerateToken(0, true, claims);

            var sessionToken = authService.GenerateToken(-1, false, claims);

            return new LogInResponse(null, sessionToken);
        }

        public async Task<UserSessionResponse> HasValidSession(bool validateAssetFilter)
        {
            var hasAsset = false;

            if (validateAssetFilter)
                hasAsset = await userRepository.HasAssetById(HttpContextHelper.GetUserId());

            return new UserSessionResponse { IsAuthenticated = true, HasAsset = hasAsset };
        }

        public async Task SendPasswordRecovery(PasswordRecoveryRequest passwordRecoveryRequest)
        {
            var user = await userRepository.GetUserByEmail(passwordRecoveryRequest.UserEmail)
                ?? throw new InvalidOperationException("UserNotFound");

            if (user.LastPasswordRecovery.HasValue && DateTime.Now.Subtract(user.LastPasswordRecovery.Value).TotalMinutes < 5)
                throw new InvalidOperationException("WaitFiveMinutesToSendEmailAgain");

            var recoveryCode = GenerateRecoveryCode();

            var mailDataRequest = new MailDataRequest
            {
                EmailToId = passwordRecoveryRequest.UserEmail,
                EmailToName = Translator.Translate("DontReply"),
                EmailSubject = Translator.Translate("PasswordRecovery"),
                EmailBody = $"{Translator.Translate("YourRecoveryCodeIs")}: {recoveryCode}",
            };

            await mailerService.SendMail(mailDataRequest, EmailBody.GetPasswordRecoveryBody(recoveryCode));

            user.LastPasswordRecovery = DateTime.Now;
            user.RecoveryCode = recoveryCode;

            await userRepository.Update(user);
        }

        public async Task RecoverPassword(NewPasswordRequest newPasswordRequest)
        {
            var user = await userRepository.GetUserByEmailAndRecoveryCode(newPasswordRequest.UserEmail, newPasswordRequest.RecoveryCode)
                ?? throw new InvalidOperationException("InvalidRecoveryCode");

            SetUserPassword(newPasswordRequest.NewPassword, ref user);

            user.UpdatedAt = DateTime.Now;

            await userRepository.Update(user);
        }

        public async Task<UserKeyResponse> GenerateKey(string document)
        {
            if (await userRepository.HasUserWithSameDocument(document))
                throw new InvalidOperationException("UserWithSameDocumentError");

            var userKey = encryptionService.EncryptDynamic($"@plasson_key2k24-{DateTime.Now}-{document}", Pwd.Pf.ToSafeValue());

            return new UserKeyResponse { UserKey = userKey };
        }

        public async Task UpdateAsset(UserAssetRequest userAssetRequest)
        {
            var user = await userRepository.GetUserIfHasAssetById(HttpContextHelper.GetUserId(), userAssetRequest.AssetId)
                ?? throw new InvalidOperationException("AssetNotFound");

            user.AssetId = userAssetRequest.AssetId;

            await userRepository.Update(user);
        }

        private static string GetSalt() => DateTime.UtcNow.Millisecond.ToString("000") + new Random().Next(100).ToString("00");

        private void SetUserPassword(string password, ref User user)
        {
            user.Salt = GetSalt();

            var keyBytes = Encoding.ASCII.GetBytes(user.Name);
            var fullPasswordBytes = Encoding.ASCII.GetBytes(password + user.Salt);

            user.Password = encryptionService.EncryptDeterministic(keyBytes, fullPasswordBytes);
        }

        private static string GenerateRecoveryCode() => DateTime.UtcNow.Ticks.ToString()[^5..];

        private string DecryptKey(string recoveryCode)
        {
            if (recoveryCode.Length != 128)
                throw new ValidationException("InvalidRecoveryCode");

            return encryptionService.DecryptDynamic(recoveryCode, Pwd.Pf.ToSafeValue());
        }
    }
}
