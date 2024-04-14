using Domain.Models.Enums.Egg;

namespace Domain.Objects.Dto_s.Egg
{
    public record EggCategoryDto
    {
        public required EEggCategory Category { get; set; }
        public string? Name { get; set; }
    }
}
