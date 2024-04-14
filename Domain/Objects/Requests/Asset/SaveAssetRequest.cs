using Domain.Utils.Helpers;
using FluentValidation;

namespace Domain.Objects.Requests.Customer
{
    public record SaveAssetRequest
    {
        public required string Name { get; set; }
        public required string Ip { get; set; }
        public int EggPackerQuantity { get; set; }
        public int DenesterQuantity { get; set; }
        public bool HasFeedback { get; set; }
        public int PlantId { get; set; }
    }

    public partial class SaveAssetRequestValidator : AbstractValidator<SaveAssetRequest>
    {
        public SaveAssetRequestValidator()
        {
            RuleFor(c => c)
                .Must(HaveValidFields);
        }

        private static bool HaveValidFields(SaveAssetRequest saveAssetRequest)
        {
            if (!saveAssetRequest.Name.IsValidLength(64))
                throw new ValidationException("InvalidName");

            if (!saveAssetRequest.Ip.IsValidLength(15))
                throw new ValidationException("InvalidIp");

            return true;
        }
    }
}
