using Domain.Models.Bases;
using Domain.Models.Enums.Scheduling;

namespace Domain.Models
{
    public class MachineSchedule : BaseSqlModel
    {
        public int MachineScheduleId { get; set; }
        public EWeekDay WeekDay { get; set; }
        public TimeSpan InitialProductionTime { get; set; }
        public TimeSpan FinalProductionTime { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int UserId { get; set; }

        #region Relationships

        public virtual ICollection<Shift>? Shifts { get; set; }
        public virtual ICollection<ScheduledStop>? ScheduledStops { get; set; }
        public required virtual User User { get; set; }

        #endregion
    }
}
