using Domain.Models.Bases;
using Domain.Models.Enums.Scheduling;

namespace Domain.Models
{
    public class Shift : BaseSqlModel
    {
        public int ShiftId { get; set; }
        public EShiftType Type { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public int MachineScheduleId { get; set; }

        #region Relationships

        public required virtual MachineSchedule MachineSchedule { get; set; }

        #endregion
    }
}

