using AutoMapper;
using Domain.Interfaces.Repositories;
using Domain.Models;
using Domain.Objects.Dto_s.Egg;
using Infra.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace Infra.Data.Repositories
{
    public class EggCategoryRepository(SqlContext context, IMapper mapper) : BaseSqlRepository<EggCategory>(context), IEggCategoryRepository
    {
        public async Task<IEnumerable<EggCategoryDto>> GetEggCategoriesDtoByUserId(int userId) =>
            await mapper.ProjectTo<EggCategoryDto>(_typedContext.AsNoTracking().Where(e => e.UserId == userId && e.Name != null)).ToListAsync();

        public async Task<IEnumerable<EggCategory>?> GetEggCategoriesByUserId(int userId) =>
            await _typedContext.Where(e => e.UserId == userId).ToListAsync();
    }
}
