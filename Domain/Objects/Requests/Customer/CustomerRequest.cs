using Domain.Utils.Helpers;
using FluentValidation;
using Microsoft.IdentityModel.Tokens;

namespace Domain.Objects.Requests.Customer
{
    public record CustomerRequest
    {
        public required string Name { get; set; }
        public required string Cnpj { get; set; }
    }

    public partial class CustomerRequestValidator : AbstractValidator<CustomerRequest>
    {
        public CustomerRequestValidator()
        {
            RuleFor(c => c)
                .Must(HaveValidFields);
        }

        private static bool HaveValidFields(CustomerRequest customerRequest)
        {
            if (customerRequest.Name.IsNullOrEmpty() || customerRequest.Name.Length > 50)
                throw new ValidationException("InvalidUsername");

            if (customerRequest.Cnpj.IsValidCnpj())
                throw new ValidationException("InvalidCnpj");

            return true;
        }
    }
}
