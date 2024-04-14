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
        public int TotvsUserId { get; set; }
        public int? AssetId { get; set; }

        #region Relationships

        public virtual ICollection<MachineSchedule>? MachineSchedules { get; set; }
        public virtual ICollection<Plant>? Plants { get; set; }
        public virtual ICollection<EggCategory>? EggCategories { get; set; }
        public virtual Asset? Asset { get; set; }

        #endregion
    }
}

