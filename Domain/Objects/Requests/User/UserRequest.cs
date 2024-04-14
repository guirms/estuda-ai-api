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
        public required string Document { get; set; }
        public required string Password { get; set; }
        public required string PasswordConfirmation { get; set; }
    }

    public partial class UserRequestValidator : AbstractValidator<UserRequest>
    {
        [GeneratedRegex(@"[!@#$%^&*(),.?""':{}|<>]")]
        private static partial Regex specialCharacterPattern();

        public UserRequestValidator()
        {
            RuleFor(u => u)
                .Must(HaveValidFields);
        }

        private bool HaveValidFields(UserRequest userRequest)
        {
            var password = userRequest.Password;
            var name = userRequest.Name;
            var document = userRequest.Document;

            if (password != userRequest.PasswordConfirmation)
                throw new ValidationException("PasswordsAreNotTheSame");

            if (name.IsNullOrEmpty() || name.Length > 50)
                throw new ValidationException("InvalidUsername");

            if (!userRequest.Email.IsValidEmail())
                throw new ValidationException("InvalidEmail");

            if (!document.IsValidCpf())
                throw new ValidationException("InvalidDocument");

            if (!IsValidPassword(password))
                throw new ValidationException("InvalidUserPassword");

            return true;
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
    }
}
