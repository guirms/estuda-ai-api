using Domain.Models.Bases;
using Domain.Models.Enums.Task;

namespace Domain.Models
{
    public class Card : BaseSqlModel
    {
        public int CardId { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public required ECardStatus TaskStatus { get; set; }
        public double Order { get; set; }
        public required TimeSpan StudyTime { get; set; }
        public int BoardId { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public required virtual Board Board { get; set; }
    }
}
