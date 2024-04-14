using AutoMapper;
using Domain.Interfaces.Repositories;
using Domain.Models;
using Domain.Objects.Dto_s.Asset;
using Infra.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace Infra.Data.Repositories
{
    public class AssetRepository(SqlContext context, IMapper mapper) : BaseSqlRepository<Asset>(context), IAssetRepository
    {
        public async Task<IEnumerable<T>?> GetAssetTypedData<T>(int currentPage, int? plantId, string? assetName, int takeQuantity = 10)
        {
            var query = _typedContext
                    .AsNoTracking()
                    .OrderByDescending(a => a.AssetId)
                    .Skip((currentPage - 1) * takeQuantity)
                    .Take(takeQuantity);

            if (plantId != null)
                query = query.Where(a => a.PlantId == plantId);

            if (assetName != null)
                query = query.Where(a => a.Name.Contains(assetName));

            return await mapper.ProjectTo<T>(query).ToListAsync();
        }
        public async Task<AssetAuthInfo?> GetKeyAndTokenById(int assetId) => mapper.Map<AssetAuthInfo>(
            await _typedContext.AsNoTracking().FirstOrDefaultAsync(a => a.AssetId == assetId));

        public async Task<int> GetPlantIdById(int assetId) => await _typedContext.AsNoTracking().Where(a => a.AssetId == assetId).Select(a => a.PlantId).FirstOrDefaultAsync();

        public async Task<bool> HasAssetById(int assetId) => await _typedContext.AsNoTracking().AnyAsync(a => a.AssetId == assetId);
    }
}
