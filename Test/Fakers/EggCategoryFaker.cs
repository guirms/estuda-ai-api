using Bogus;
using Domain.Objects.Requests.Egg;

namespace Test.Fakers
{
    internal static class EggCategoryFaker
    {
        internal static IEnumerable<EggCategoriesRequest> GenerateDefaultEggCategories(int count = 1, int namesLength = 8)
        {
            return new Faker<EggCategoriesRequest>()
                .RuleFor(e => e.Name, f => f.Name.Random.String2(namesLength))
                .Generate(count);
        }
    }
}
