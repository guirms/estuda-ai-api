using Domain.Utils.Helpers;
using FluentValidation;

namespace Domain.Objects.Requests.Customer
{
    public record SaveLayoutRequest
    {
        public required string Name { get; set; }
        public string? Ip { get; set; }
        public int ProductId { get; set; }
    }

    public partial class SaveLayoutRequestValidator : AbstractValidator<SaveLayoutRequest>
    {
        public SaveLayoutRequestValidator()
        {
            RuleFor(s => s)
                .Must(HaveValidFields);
        }

        private static bool HaveValidFields(SaveLayoutRequest saveLayoutRequest)
        {
            if (!saveLayoutRequest.Name.IsValidLength(64))
                throw new ValidationException("InvalidName");

            if (!saveLayoutRequest.Ip.IsValidLength(15))
                throw new ValidationException("InvalidIp");

            return true;
        }
    }
}
