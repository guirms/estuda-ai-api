using AutoMapper;
using Domain.Interfaces.Repositories;
using Domain.Models;
using Domain.Objects.Requests.Plant;
using Infra.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace Infra.Data.Repositories
{
    public class PlantRepository(SqlContext context, IMapper mapper) : BaseSqlRepository<Plant>(context), IPlantRepository
    {
        public async Task<PlantSchema?> GetSchemaById(int plantId, int assetId)
        {
            return mapper.Map<PlantSchema>(await _typedContext
                .AsNoTracking()
                .Include(p => p.Assets!.Where(a => a.AssetId == assetId))
                .ThenInclude(a => a.Products!)
                .ThenInclude(p => p.Layouts)
                .Where(p => p.PlantId == plantId)
                .FirstOrDefaultAsync());
        }

        public async Task<IEnumerable<T>?> GetPlantTypedData<T>(int currentPage, string? plantName, string? plantCnpj, int? userId = null,
            int takeQuantity = 10)
        {
            var query = _typedContext
                    .AsNoTracking()
                    .OrderByDescending(p => p.PlantId)
                    .Skip((currentPage - 1) * takeQuantity)
                    .Take(takeQuantity);

            if (userId != null)
                query = query.Where(p => p.UserId == userId);

            if (plantName != null)
                query = query.Where(p => p.Name.Contains(plantName));

            if (plantCnpj != null)
                query = query.Where(p => p.Cnpj.Contains(plantCnpj));

            return await mapper.ProjectTo<T>(query).ToListAsync();
        }

        public async Task<bool> HasPlantById(int plantId) => await _typedContext.AsNoTracking().AnyAsync(p => p.PlantId == plantId);

        public async Task HasPlantWithTheSameInfo(string? cnpj, string? name, int? plantId = null)
        {
            if (await _typedContext.AsNoTracking().AnyAsync(p => p.Cnpj == cnpj && p.PlantId != plantId))
                throw new InvalidOperationException("PlantWithSameCnpjError");

            if (await _typedContext.AsNoTracking().AnyAsync(p => p.Name == name && p.PlantId != plantId))
                throw new InvalidOperationException("PlantWithSameNameError");
        }
    }
}
