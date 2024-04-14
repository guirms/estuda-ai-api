using AutoMapper;
using Domain.Interfaces.Repositories;
using Domain.Interfaces.Services;
using Domain.Models;
using Domain.Objects.Requests.User;
using Domain.Objects.Responses.Asset;
using Domain.Utils.Languages;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Domain.Services
{
    public class UserService(IAuthService authService, IEncryptionService encryptionService, IUserRepository userRepository, IMapper mapper) : IUserService
    {
        public async Task<IEnumerable<UserResultsResponse>?> Get(int currentPage, string? userName) => await userRepository.GetUserResults(currentPage, userName);

        public async Task<string> Save(UserRequest userRequest)
        {
            await userRepository.HasUserWithTheSameInfo(userRequest);

            var user = mapper.Map<User>(userRequest);

            SetUserPassword(user.Password, ref user);

            await userRepository.Save(user);

            return Translator.Translate("UserSavedSuccessfully");
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

            var userToken = authService.GenerateToken(user.UserId);

            if (userToken.IsNullOrEmpty())
                throw new InvalidOperationException("ErrorLoggingIn");

            await userRepository.Update(user);

            return new LogInResponse(userToken);
        }

        private static string GetSalt() => DateTime.UtcNow.Millisecond.ToString("000") + new Random().Next(100).ToString("00");

        private void SetUserPassword(string password, ref User user)
        {
            user.Salt = GetSalt();

            var keyBytes = Encoding.ASCII.GetBytes(user.Name);
            var fullPasswordBytes = Encoding.ASCII.GetBytes(password + user.Salt);

            user.Password = encryptionService.EncryptDeterministic(keyBytes, fullPasswordBytes);
        }
    }
}
