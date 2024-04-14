using Domain.Objects.Requests.Egg;
using Domain.Services;
using Test.Setup;
using Xunit;

namespace Test.Fixtures
{
    [CollectionDefinition(nameof(EggCategoryFixture))]
    public class EggCategoryFixture : TestSetup
    {
        protected readonly EggCategoryService _eggCategoryService;
        protected readonly EggCategoriesRequestValidator _eggCategoriesRequestValidator = new();

        //internal EggCategoryFixture()
        //{
        //    _eggCategoryService = new EggCategoryService(CreateMock<IEggCategoryRepository>().Object, CreateMock<IHttpContextAccessor>().Object);
        //}
    }
}
