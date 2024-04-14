using Domain.Models.Bases;
using Domain.Models.Enums.Egg;

namespace Domain.Models
{
    public class EggCategory : BaseSqlModel
    {
        public int EggCategoryId { get; set; }
        public required EEggCategory Category { get; set; }
        public string? Name { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int UserId { get; set; }

        #region Relationships

        public required virtual User User { get; set; }

        #endregion
    }
}
