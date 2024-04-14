using Domain.Interfaces.Services;
using Domain.Utils.Constants;
using Domain.Utils.Helpers;
using FluentValidation;
using Microsoft.IdentityModel.Tokens;
using System.Text.RegularExpressions;

namespace Domain.Objects.Requests.User
{
    public record UserRequest
    {
        public required string Name { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
        public required string PasswordConfirmation { get; set; }
        public required string UserKey { get; set; }
    }

    public partial class UserRequestValidator : AbstractValidator<UserRequest>
    {
        private readonly IEncryptionService _encryptionService;

        [GeneratedRegex(@"[!@#$%^&*(),.?""':{}|<>]")]
        private static partial Regex specialCharacterPattern();

        public UserRequestValidator(IEncryptionService encryptionService)
        {
            _encryptionService = encryptionService;

            RuleFor(u => u)
                .Must(HaveValidFields);
        }

        public static bool IsValidPassword(string password)
        {
            if (password.IsNullOrEmpty() || password.Length < 8 || password.Length > 50)
                throw new ValidationException("PasswordMustHaveValidLength");

            if (!specialCharacterPattern().IsMatch(password))
                throw new ValidationException("PasswordMustHaveSpecialCharacter");

            if (!password.Any(char.IsLower) || !password.Any(char.IsUpper))
                throw new ValidationException("PasswordMustContainUppercaseAndLowercase");

            return true;
        }

        private bool HaveValidFields(UserRequest userRequest)
        {
            var password = userRequest.Password;
            var name = userRequest.Name;

            if (password != userRequest.PasswordConfirmation)
                throw new ValidationException("PasswordsAreNotTheSame");

            if (!IsValidPassword(password))
                throw new ValidationException("InvalidUserPassword");

            if (!userRequest.Email.IsValidEmail())
                throw new ValidationException("InvalidEmail");

            if (!IsValidUserKey(userRequest.UserKey))
                throw new ValidationException("InvalidLicenseKey");

            if (name.IsNullOrEmpty() || name.Length > 50)
                throw new ValidationException("InvalidUsername");

            return true;
        }

        private bool IsValidUserKey(string userKey)
        {
            try
            {
                var decryptedUserKey = _encryptionService.DecryptDynamic(userKey, Pwd.Pf).Split("-");

                if (decryptedUserKey.Length != 3 || decryptedUserKey[0] != "@plasson_key2k24")
                    throw new ValidationException("InvalidLicenseKey");

                if (!decryptedUserKey[2].IsValidDocument())
                    throw new ValidationException("InvalidLicenseKey");

                var keyCreationDateTime = DateTime.Parse(decryptedUserKey[1]);

                if (DateTime.Now.Subtract(keyCreationDateTime).TotalMinutes > 60)
                    throw new ValidationException("InvalidLicenseKey");

                return true;
            }
            catch
            {
                throw new ValidationException("InvalidLicenseKey");
            }
        }
    }
}
