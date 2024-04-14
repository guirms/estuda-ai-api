using Domain.Models.Enums.Egg;
using FluentValidation;

namespace Domain.Objects.Requests.Egg
{
    public record EggCategoriesRequest
    {
        public required EEggCategory Category { get; set; }
        public string? Name { get; set; }
    }

    public class EggCategoriesRequestValidator : AbstractValidator<IEnumerable<EggCategoriesRequest>>
    {
        public EggCategoriesRequestValidator()
        {
            RuleFor(e => e
                .Where(e => e.Name != null)
                .Select(e => e.Name))
                .Must(HaveValidWordLength);

            RuleFor(e => e)
                .Must(HaveValidListLength);
        }

        private static bool HaveValidWordLength(IEnumerable<string?> wordList)
        {
            if (wordList != null)
                foreach (var word in wordList)
                {
                    if (word?.Length > 15)
                        throw new ValidationException("StringLengthValidation");
                }

            return true;
        }

        private static bool HaveValidListLength(IEnumerable<EggCategoriesRequest> eggCategoriesRequest)
        {
            var eggCategoriesLength = eggCategoriesRequest.Count();

            if (eggCategoriesLength < 1 || eggCategoriesLength > 7)
                throw new ValidationException("InvalidCategoryList");

            return true;
        }
    }
}
