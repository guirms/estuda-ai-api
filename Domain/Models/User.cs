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
        public bool IsBatchDisabled { get; set; }
        public required string Key { get; set; }
        public DateTime? LastLogin { get; set; }
        public DateTime? LastPasswordRecovery { get; set; }
        public string? RecoveryCode { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}

