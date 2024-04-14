using FluentValidation;
using FluentValidation.TestHelper;
using Test.Fakers;
using Test.Fixtures;
using Xunit;

namespace Test.Tests.Validators
{
    public class EggCategoriesRequestValidatorTest : EggCategoryFixture
    {
        private const string TraitName = "EggCategoriesRequest";
        private const string HaveValidLength = "HaveValidLength";

        [Fact(DisplayName = "Name with more than 15 characters")]
        [Trait(TraitName, HaveValidLength)]
        internal void NameWithMoreThan15Characters_ThrowsStringError()
        {
            var methodCall = Assert.Throws<ValidationException>(() =>
                _eggCategoriesRequestValidator.TestValidate(EggCategoryFaker.GenerateDefaultEggCategories(5, 16)));

            Assert.Equal("StringLengthValidation", methodCall.Message);
        }
    }

}
