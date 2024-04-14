using Domain.Models;
using Domain.Objects.Requests.Plant;

namespace Domain.Interfaces.Repositories
{
    public interface IPlantRepository : IBaseSqlRepository<Plant>
    {
        Task<PlantSchema?> GetSchemaById(int plantId, int assetId);
        Task<IEnumerable<T>?> GetPlantTypedData<T>(int currentPage, string? plantName, string? plantCnpj, int? userId = null, int takeQuantity = 10);
        Task<bool> HasPlantById(int plantId);
        Task HasPlantWithTheSameInfo(string? cnpj, string? name, int? plantId = null);
    }
}
