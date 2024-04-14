using Domain.Models;
using Domain.Objects.Dto_s.Egg;

namespace Domain.Interfaces.Repositories
{
    public interface IEggCategoryRepository : IBaseSqlRepository<EggCategory>
    {
        Task<IEnumerable<EggCategoryDto>> GetEggCategoriesDtoByUserId(int userId);
        Task<IEnumerable<EggCategory>?> GetEggCategoriesByUserId(int userId);
    }
}
