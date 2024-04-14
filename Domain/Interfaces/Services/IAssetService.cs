using Domain.Objects.Requests.Customer;
using Domain.Objects.Responses.Asset;

namespace Domain.Interfaces.Services
{
    public interface IAssetService
    {
        Task<IEnumerable<AssetToTableResponse>?> GetToTable(int currentPage, string? assetName);
        Task<IEnumerable<AssetToFilterResponse>?> GetToFilter(int currentPage, int plantId, string? assetName);
        Task Save(SaveAssetRequest saveAssetRequest);
        Task Update(UpdateAssetRequest updateAssetRequest);
        Task Delete(int assetId);
        Task<AssetKeyResponse> GenerateKey(int assetId);
        Task<AssetConfigResponse> GetConfig();
        Task DeleteAuthToken();
    }
}