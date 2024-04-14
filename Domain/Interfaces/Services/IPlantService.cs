using Domain.Objects.Requests.Customer;
using Domain.Objects.Requests.Plant;
using Domain.Objects.Responses.Machine;

namespace Domain.Interfaces.Services
{
    public interface IPlantService
    {
        Task<IEnumerable<PlantToTableResponse>?> GetToTable(int currentPage, string? plantName, string? plantCnpj);
        Task<PlantToFilterResponse?> GetToFilter(int currentPage, string? plantName, string? plantCnpj);
        Task Save(SavePlantRequest savePlantRequest);
        Task Update(UpdatePlantRequest updatePlantRequest);
        Task Delete(int plantId);
        Task<PlantSchema> GetSchema(int plantId, int assetId);
    }
}