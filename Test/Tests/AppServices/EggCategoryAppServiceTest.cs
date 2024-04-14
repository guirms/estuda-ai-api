using Test.Fixtures;

namespace Test.Tests.AppServices
{
    public class EggCategoryAppServiceTest : EggCategoryFixture
    {
        private const string TraitName = "EggCategoryAppService";
        private const string Update = "Update";

        //[Fact(DisplayName = "Empty egg categories from database")]
        //[Trait(TraitName, Update)]
        //internal async void EmptyEggCategoriesFromDatabase_ThrowsNoCategoriesFoundError()
        //{
        //    var methodCall = await Assert.ThrowsAsync<InvalidOperationException>(() =>
        //        _eggCategoryAppService.Update(Any<List<EggCategoriesRequest>>()));

        //    Assert.Equal("NoEggCategoriesFound", methodCall.Message);
        //}
    }
}
