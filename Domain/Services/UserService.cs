using AutoMapper;
using Domain.Interfaces.Repositories;
using Domain.Interfaces.Services;
using Domain.Models;
using Domain.Objects.Requests.User;
using Domain.Objects.Responses.Asset;
using Domain.Utils.Constants;
using Domain.Utils.Helpers;
using FluentValidation;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

namespace Domain.Services
{
    public class UserService(IAuthService authService, IEncryptionService encryptionService, IUserRepository userRepository, IMapper mapper) : IUserService
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

        private static string GetSalt() => DateTime.UtcNow.Millisecond.ToString("000") + new Random().Next(100).ToString("00");

        private void SetUserPassword(string password, ref User user)
        {
            user.Salt = GetSalt();

            var keyBytes = Encoding.ASCII.GetBytes(user.Name);
            var fullPasswordBytes = Encoding.ASCII.GetBytes(password + user.Salt);

            user.Password = encryptionService.EncryptDeterministic(keyBytes, fullPasswordBytes);
        }

        private string DecryptKey(string recoveryCode)
        {
            if (recoveryCode.Length != 128)
                throw new ValidationException("InvalidRecoveryCode");

            return encryptionService.DecryptDynamic(recoveryCode, Pwd.Pf.ToSafeValue());
        }
    }
}
