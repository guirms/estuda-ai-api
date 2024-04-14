using Domain.Models;
using Domain.Objects.Dto_s.Asset;

namespace Domain.Interfaces.Repositories
{
    public interface IAssetRepository : IBaseSqlRepository<Asset>
    {
        Task<IEnumerable<T>?> GetAssetTypedData<T>(int currentPage, int? plantId, string? assetName, int takeQuantity = 10);
        Task<AssetAuthInfo?> GetKeyAndTokenById(int assetId);
        Task<int> GetPlantIdById(int assetId);
        Task<bool> HasAssetById(int assetId);
    }
}
