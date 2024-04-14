using Domain.Models.Bases;

namespace Domain.Models
{
    public class ScheduledStop : BaseSqlModel
    {
        public int ScheduledStopId { get; set; }
        public required string Name { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public int MachineScheduleId { get; set; }

        #region Relationships

        public required virtual MachineSchedule MachineSchedule { get; set; }

        #endregion
    }
}
