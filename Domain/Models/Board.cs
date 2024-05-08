using Domain.Models.Bases;

namespace Domain.Models
{
    public class Board : BaseSqlModel
    {
        public int BoardId { get; set; }
        public required string Name { get; set; }
        public required DateTime ExamDateTime { get; set; }
        public required TimeSpan DailyStudyTime { get; set; }
        public int UserId { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public required virtual User User { get; set; }
        public virtual IEnumerable<Card>? Cards { get; set; }
    }
}

