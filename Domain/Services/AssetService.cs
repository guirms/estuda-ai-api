using AutoMapper;
using Domain.Interfaces.Repositories;
using Domain.Interfaces.Services;
using Domain.Models;
using Domain.Objects.Requests.Customer;
using Domain.Objects.Responses.Asset;
using Domain.Utils.Constants;
using Domain.Utils.Helpers;
using System.Security.Claims;

namespace Domain.Services
{
    public class AssetService(IPlantService plantService, IAuthService authService, IAssetRepository assetRepository, IPlantRepository plantRepository, IMapper mapper) : IAssetService
    {
        public async Task<IEnumerable<AssetToTableResponse>?> GetToTable(int currentPage, string? assetName) => await assetRepository.GetAssetTypedData<AssetToTableResponse>(currentPage, null, assetName);

        public async Task<IEnumerable<AssetToFilterResponse>?> GetToFilter(int currentPage, int plantId, string? assetName) => await assetRepository.GetAssetTypedData<AssetToFilterResponse>(currentPage, plantId, assetName);

        public async Task Save(SaveAssetRequest saveAssetRequest)
        {
            if (!await plantRepository.HasPlantById(saveAssetRequest.PlantId))
                throw new InvalidOperationException("PlantNotFound");

            await assetRepository.Save(mapper.Map<Asset>(saveAssetRequest));
        }

        public async Task Update(UpdateAssetRequest updateAssetRequest)
        {
            var asset = await assetRepository.GetById(updateAssetRequest.AssetId)
                        ?? throw new InvalidOperationException("AssetNotFound");

            asset = updateAssetRequest.MapIgnoringNullProperties(asset);
            asset.UpdatedAt = DateTime.Now;

            await assetRepository.Update(asset);
        }

        public async Task Delete(int assetId) => await assetRepository.Delete(assetId);

        public async Task<AssetKeyResponse> GenerateKey(int assetId)
        {
            var assetKeyAndToken = await assetRepository.GetKeyAndTokenById(assetId);

            if (assetKeyAndToken != null && assetKeyAndToken.Key != null)
            {
                if (assetKeyAndToken.AuthToken != null)
                    throw new InvalidOperationException("AssetAlreadyLicensed");

                return new AssetKeyResponse { AssetKey = assetKeyAndToken.Key };
            }

            var claims = new List<Claim>
            {
                new("assetId", assetId.ToString()),
                new(Token.ClaimPassword, Pwd.Auth)
            };

            var assetKey = authService.GenerateToken(0, false, claims, DateTime.UtcNow.AddDays(7));

            var asset = await assetRepository.GetById(assetId)
                ?? throw new InvalidOperationException("AssetNotFound");

            asset.Key = assetKey;
            asset.UpdatedAt = DateTime.Now;

            await assetRepository.Update(asset);

            return new AssetKeyResponse { AssetKey = assetKey };
        }

        public async Task<AssetConfigResponse> GetConfig()
        {
            var assetId = HttpContextHelper.GetClaimValueFromHeaderToken("assetId")?.ToIntSafe()
                ?? throw new InvalidOperationException("ErrorGettingSessionInfo");

            var asset = await assetRepository.GetById(assetId) ??
                throw new InvalidOperationException("AssetNotFound");

            if (asset.AuthToken != null)
                throw new InvalidOperationException("AssetAlreadyLicensed");

            var claims = new List<Claim>
            {
                new(Token.ClaimPassword, Pwd.Auth),
                new(Token.AssetPassword, Pwd.Asset)
            };

            var assetAuthToken = authService.GenerateToken(0, false, claims, DateTime.UtcNow.AddDays(365000));

            asset.AuthToken = assetAuthToken;
            asset.UpdatedAt = DateTime.Now;

            await assetRepository.Update(asset);

            var plantSchema = await plantService.GetSchema(asset.PlantId, asset.AssetId);

            return new AssetConfigResponse { AssetAuthToken = assetAuthToken, PlantSchema = plantSchema };
        }

        public async Task DeleteAuthToken()
        {
            var assetId = HttpContextHelper.GetClaimValueFromHeaderToken("assetId")?.ToIntSafe()
                ?? throw new InvalidOperationException("ErrorGettingSessionInfo");

            var asset = await assetRepository.GetById(assetId) ??
            throw new InvalidOperationException("AssetNotFound");

            if (asset.AuthToken == null)
                throw new InvalidOperationException("AssetNotFound");

            asset.AuthToken = null;

            await assetRepository.Update(asset);
        }
    }
}
