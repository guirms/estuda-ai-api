using Domain.Models.Enums.Product;
using Domain.Utils.Helpers;
using FluentValidation;

namespace Domain.Objects.Requests.Customer
{
    public record SaveProductRequest
    {
        public required string Name { get; set; }
        public EProductType ProductType { get; set; }
        public int AssetId { get; set; }
    }

    public partial class SaveProductRequestValidator : AbstractValidator<SaveProductRequest>
    {
        public SaveProductRequestValidator()
        {
            RuleFor(s => s)
                .Must(HaveValidFields);
        }

        private static bool HaveValidFields(SaveProductRequest saveProductRequest)
        {
            if (!saveProductRequest.Name.IsValidLength(64))
                throw new ValidationException("InvalidName");

            if (!saveProductRequest.ProductType.IsValidEnumValue())
                throw new ValidationException("ModelNotFound");

            return true;
        }
    }
}
