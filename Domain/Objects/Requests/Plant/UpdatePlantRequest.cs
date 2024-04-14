using Domain.Utils.Helpers;
using FluentValidation;

namespace Domain.Objects.Requests.Customer
{
    public record UpdatePlantRequest
    {
        public int PlantId { get; set; }
        public string? Name { get; set; }
        public string? Cnpj { get; set; }
        public string? Address { get; set; }
        public string? ZipCode { get; set; }
        public string? Latitude { get; set; }
        public string? Longitude { get; set; }
    }

    public partial class UpdatePlantRequestValidator : AbstractValidator<UpdatePlantRequest>
    {
        public UpdatePlantRequestValidator()
        {
            RuleFor(s => s)
                .Must(HaveValidFields);
        }

        private static bool HaveValidFields(UpdatePlantRequest updatePlantRequest)
        {
            var name = updatePlantRequest.Name;
            var zIpCode = updatePlantRequest.ZipCode;
            var address = updatePlantRequest.Address;
            var cnpj = updatePlantRequest.Cnpj;
            var latitude = updatePlantRequest.Latitude;
            var longitude = updatePlantRequest.Longitude;

            if (name != null && !name.IsValidLength(64))
                throw new ValidationException("InvalidName");

            if (zIpCode != null && !zIpCode.IsValidLength(16))
                throw new ValidationException("InvalidZipCode");

            if (address != null && !address.IsValidLength(256))
                throw new ValidationException("InvalidZipCode");

            if (cnpj != null && !cnpj.IsValidCnpj())
                throw new ValidationException("InvalidCnpj");

            if ((latitude != null && !latitude.IsValidLatitude()) || (longitude != null && !longitude.IsValidLongitude()))
                throw new ValidationException("InvalidLatitudeLongitude");

            return true;
        }
    }
}
