using Domain.Utils.Helpers;
using FluentValidation;

namespace Domain.Objects.Requests.User
{
    public record PasswordRecoveryRequest
    {
        public required string UserEmail { get; set; }
    }

    public class PasswordRecoveryRequestValidator : AbstractValidator<PasswordRecoveryRequest>
    {
        public PasswordRecoveryRequestValidator()
        {
            RuleFor(p => p)
                .Must(HaveValidFields);
        }

        private bool HaveValidFields(PasswordRecoveryRequest passwordRecoveryRequest)
        {
            if (!passwordRecoveryRequest.UserEmail.IsValidEmail())
                throw new ValidationException("InvalidEmail");

            return true;
        }
    }
}
