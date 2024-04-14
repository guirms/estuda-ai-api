using Domain.Interfaces.Services;
using FluentValidation;

namespace Domain.Objects.Requests.User
{
    public record NewPasswordRequest
    {
        public required string RecoveryCode { get; set; }
        public required string UserEmail { get; set; }
        public required string NewPassword { get; set; }
        public required string NewPasswordConfirmation { get; set; }
    }

    public class NewPasswordRequestValidator : AbstractValidator<NewPasswordRequest>
    {
        public NewPasswordRequestValidator(IEncryptionService encryptionService)
        {
            RuleFor(u => u)
                .Must(HaveValidFields);
        }

        private bool HaveValidFields(NewPasswordRequest newPasswordRequest)
        {
            var newPassword = newPasswordRequest.NewPassword;
            var newPasswordConfirmation = newPasswordRequest.NewPasswordConfirmation;

            if (newPassword != newPasswordConfirmation)
                throw new ValidationException("PasswordsAreNotTheSame");

            if (!UserRequestValidator.IsValidPassword(newPassword))
                throw new ValidationException("InvalidUserPassword");

            if (newPasswordRequest.RecoveryCode.Length != 5)
                throw new ValidationException("InvalidRecoveryCode");

            return true;
        }
    }
}
