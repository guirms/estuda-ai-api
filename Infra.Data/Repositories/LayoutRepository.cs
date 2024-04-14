using AutoMapper;
using Domain.Interfaces.Repositories;
using Domain.Models;
using Infra.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace Infra.Data.Repositories
{
    public class LayoutRepository(SqlContext context, IMapper mapper) : BaseSqlRepository<Layout>(context), ILayoutRepository
    {
        public async Task<IEnumerable<T>?> GetLayoutTypedData<T>(int currentPage, string? layoutName, int takeQuantity = 10)
        {
            var query = _typedContext
                    .AsNoTracking()
                    .OrderByDescending(p => p.LayoutId)
                    .Skip((currentPage - 1) * takeQuantity)
                    .Take(takeQuantity);

            if (layoutName != null)
                query = query.Where(p => p.Name.Contains(layoutName));

            return await mapper.ProjectTo<T>(query).ToListAsync();
        }
    }
}
