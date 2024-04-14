using Domain.Utils.Helpers;
using FluentValidation;

namespace Domain.Objects.Requests.Customer
{
    public record UpdateAssetRequest
    {
        public int AssetId { get; set; }
        public string? Name { get; set; }
        public string? Ip { get; set; }
        public int? EggPackerQuantity { get; set; }
        public int? DenesterQuantity { get; set; }
        public bool? HasFeedback { get; set; }
    }

    public partial class UpdateAssetRequestValidator : AbstractValidator<UpdateAssetRequest>
    {
        public UpdateAssetRequestValidator()
        {
            RuleFor(c => c)
                .Must(HaveValidFields);
        }

        private static bool HaveValidFields(UpdateAssetRequest updateAssetRequest)
        {
            var name = updateAssetRequest.Name;
            var ip = updateAssetRequest.Ip;

            if (name != null && !name.IsValidLength(64))
                throw new ValidationException("InvalidName");

            if (ip != null && !ip.IsValidLength(15))
                throw new ValidationException("InvalidIp");

            return true;
        }
    }
}
