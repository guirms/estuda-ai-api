using Domain.Models.Bases;
using Domain.Models.Enums.Egg;
using Domain.Models.Enums.Machine;
using Domain.Models.Enums.Scheduling;

namespace Domain.Models
{
    public class MachineOperation : BaseNoSqlModel
    {
        public EMachineStatus MachineStatus { get; set; }
        public EDevStatus DevStatus { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public int TotalProduction { get; set; }
        public int TotalBad { get; set; }
        public int TotalWhite { get; set; }
        public int TotalWhiteBad { get; set; }
        public int TotalDirty { get; set; }
        public int TotalCracked { get; set; }
        public int TotalBroken { get; set; }
        public int TotalLeaked { get; set; }
        public double? CurrentSpeed { get; set; }
        public double? ProgSpeed { get; set; }
        public double? Fill { get; set; }
        public double? Flow { get; set; }
        public EShiftType? ShiftType { get; set; }
        public int MachineScheduleId { get; set; }
        public int AssetId { get; set; }
        public int? CustomerId { get; set; }
        public required ICollection<EggQuantity> EggQuantities { get; set; }
    }

    public class EggQuantity
    {
        public EEggQuantityType Type { get; set; }
        public int P1 { get; set; }
        public int P2 { get; set; }
        public int P3 { get; set; }
        public int P4 { get; set; }
        public int P5 { get; set; }
        public int P6 { get; set; }
        public int P7 { get; set; }
    }
}