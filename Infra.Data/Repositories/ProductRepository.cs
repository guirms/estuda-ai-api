using AutoMapper;
using Domain.Interfaces.Repositories;
using Domain.Models;
using Infra.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace Infra.Data.Repositories
{
    public class ProductRepository(SqlContext context, IMapper mapper) : BaseSqlRepository<Product>(context), IProductRepository
    {
        public async Task<IEnumerable<T>?> GetProductTypedData<T>(int currentPage, string? productName, int takeQuantity = 10)
        {
            var query = _typedContext
                    .AsNoTracking()
                    .OrderByDescending(p => p.ProductId)
                    .Skip((currentPage - 1) * takeQuantity)
                    .Take(takeQuantity);

            if (productName != null)
                query = query.Where(p => p.Name.Contains(productName));

            return await mapper.ProjectTo<T>(query).ToListAsync();
        }

        public async Task<bool> HasProductById(int productId) => await _typedContext.AsNoTracking().AnyAsync(p => p.ProductId == productId);
    }
}
