using Domain.Utils.Helpers;
using FluentValidation;

namespace Domain.Objects.Requests.Customer
{
    public record UpdateLayoutRequest
    {
        public int LayoutId { get; set; }
        public string? Ip { get; set; }
        public string? Name { get; set; }
    }

    public partial class UpdateLayoutRequestValidator : AbstractValidator<UpdateLayoutRequest>
    {
        public UpdateLayoutRequestValidator()
        {
            RuleFor(s => s)
                .Must(HaveValidFields);
        }

        private static bool HaveValidFields(UpdateLayoutRequest updateLayoutRequest)
        {
            var name = updateLayoutRequest.Name;
            var ip = updateLayoutRequest.Ip;

            if (name != null && !name.IsValidLength(64))
                throw new ValidationException("InvalidName");

            if (ip != null && !ip.IsValidLength(15))
                throw new ValidationException("InvalidIp");

            return true;
        }
    }
}
