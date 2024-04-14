using Domain.Models;

namespace Domain.Interfaces.Repositories
{
    public interface IProductRepository : IBaseSqlRepository<Product>
    {
        Task<IEnumerable<T>?> GetProductTypedData<T>(int currentPage, string? productName, int takeQuantity = 10);
        Task<bool> HasProductById(int productId);
    }
}
