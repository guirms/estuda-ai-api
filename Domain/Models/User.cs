using Domain.Models.Bases;

namespace Domain.Models
{
    public class User : BaseSqlModel
    {
        public int UserId { get; set; }
        public required string Name { get; set; }
        public required string Email { get; set; }
        public required string Document { get; set; }
        public required string Password { get; set; }
        public required string Salt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}

