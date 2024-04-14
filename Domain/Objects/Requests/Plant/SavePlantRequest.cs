using Domain.Utils.Helpers;
using FluentValidation;

namespace Domain.Objects.Requests.Customer
{
    public record SavePlantRequest
    {
        public required string Name { get; set; }
        public required string Cnpj { get; set; }
        public required string Address { get; set; }
        public required string ZipCode { get; set; }
        public required string Latitude { get; set; }
        public required string Longitude { get; set; }
        public int UserId { get; set; }
        public IEnumerable<SaveAssetRequest>? Assets { get; set; }
    }

    public partial class SavePlantRequestValidator : AbstractValidator<SavePlantRequest>
    {
        public SavePlantRequestValidator()
        {
            RuleFor(s => s)
                .Must(HaveValidFields);
        }

        private static bool HaveValidFields(SavePlantRequest savePlantRequest)
        {
            if (!savePlantRequest.Name.IsValidLength(64))
                throw new ValidationException("InvalidName");

            if (!savePlantRequest.Cnpj.IsValidCnpj())
                throw new ValidationException("InvalidCnpj");

            if (!savePlantRequest.Address.IsValidLength(256))
                throw new ValidationException("InvalidZipCode");

            if (!savePlantRequest.ZipCode.IsValidLength(16))
                throw new ValidationException("InvalidZipCode");

            if (!savePlantRequest.Latitude.IsValidLatitude() || !savePlantRequest.Longitude.IsValidLongitude())
                throw new ValidationException("InvalidLatitudeLongitude");

            return true;
        }
    }
}
