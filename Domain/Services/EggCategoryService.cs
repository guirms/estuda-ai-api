using Domain.Interfaces.Repositories;
using Domain.Interfaces.Services;
using Domain.Objects.Requests.Egg;
using Domain.Utils.Helpers;

namespace Domain.Services
{
    public class EggCategoryService(IEggCategoryRepository eggCategoryRepository) : IEggCategoryService
    {
        public async Task Update(IEnumerable<EggCategoriesRequest> eggCategoriesRequest)
        {
            var eggCategories = await eggCategoryRepository.GetEggCategoriesByUserId(HttpContextHelper.GetUserId()) ??
                throw new InvalidOperationException("NoEggCategoriesFound");

            if (!eggCategories.Any())
                throw new InvalidOperationException("NoEggCategoriesFound");

            foreach (var eggCategoryDb in eggCategories)
            {
                foreach (var eggCategoryReq in eggCategoriesRequest)
                {
                    if (eggCategoryDb.Category == eggCategoryReq.Category)
                    {
                        eggCategoryDb.Name = eggCategoryReq.Name;
                        eggCategoryDb.UpdatedAt = DateTime.Now;
                    }
                }
            }

            await eggCategoryRepository.UpdateMany(eggCategories);
        }
    }
}
