using Domain.Objects.Requests.Egg;
using Test.Fixtures;
using Xunit;

namespace Test.Tests.Services
{
    public class EggCategoryServiceTest : EggCategoryFixture
    {
        private const string TraitName = "EggCategoryService";
        private const string Update = "Update";

        [Fact(DisplayName = "Empty egg categories from database")]
        [Trait(TraitName, Update)]
        internal async void EmptyEggCategoriesFromDatabase_ThrowsNoCategoriesFoundError()
        {
            var methodCall = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _eggCategoryService.Update(Any<List<EggCategoriesRequest>>()));

            Assert.Equal("NoEggCategoriesFound", methodCall.Message);
        }
    }
}
