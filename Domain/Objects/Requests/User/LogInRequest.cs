using Domain.Utils.Helpers;
using FluentValidation;
using Microsoft.IdentityModel.Tokens;

namespace Domain.Objects.Requests.User
{
    public record LogInRequest
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
        public bool RememberMe { get; set; }
    }

    public class LogInRequestValidator : AbstractValidator<LogInRequest>
    {
        public LogInRequestValidator()
        {
            RuleFor(u => u)
                .Must(HaveValidFields);
        }

        private static bool HaveValidFields(LogInRequest logInRequest)
        {
            if (logInRequest.Email.IsNullOrEmpty() || !logInRequest.Email.IsValidEmail())
                throw new ValidationException("InvalidEmail");

            if (logInRequest.Password.IsNullOrEmpty() || logInRequest.Password.Length > 50)
                throw new ValidationException("InvalidUserPassword");

            return true;
        }
    }
}
