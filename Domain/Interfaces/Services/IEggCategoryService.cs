using Domain.Objects.Requests.Egg;

namespace Domain.Interfaces.Services
{
    public interface IEggCategoryService
    {
        Task Update(IEnumerable<EggCategoriesRequest> eggCategoriesRequest);
    }
}
