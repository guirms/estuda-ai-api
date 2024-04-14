using Domain.Models;

namespace Domain.Interfaces.Repositories
{
    public interface ILayoutRepository : IBaseSqlRepository<Layout>
    {
        Task<IEnumerable<T>?> GetLayoutTypedData<T>(int currentPage, string? layoutName, int takeQuantity = 10);
    }
}
