using Domain.Models.Enums.Product;
using Domain.Utils.Helpers;
using FluentValidation;

namespace Domain.Objects.Requests.Customer
{
    public record UpdateProductRequest
    {
        public int ProductId { get; set; }
        public string? Name { get; set; }
        public EProductType? ProductType { get; set; }
    }

    public partial class UpdateProductRequestValidator : AbstractValidator<UpdateProductRequest>
    {
        public UpdateProductRequestValidator()
        {
            RuleFor(s => s)
                .Must(HaveValidFields);
        }

        private static bool HaveValidFields(UpdateProductRequest updateProductRequest)
        {
            var name = updateProductRequest.Name;

            if (name != null && !name.IsValidLength(64))
                throw new ValidationException("InvalidName");

            return true;
        }
    }
}
