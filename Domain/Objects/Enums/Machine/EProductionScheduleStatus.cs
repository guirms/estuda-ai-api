using System.ComponentModel;

namespace Domain.Objects.Enums.Machine
{
    public enum EProductionScheduleStatus
    {
        [Description("#8a8888")]
        Off,
        [Description("#E2211C")]
        Emergency,
        [Description("#6EF233")]
        Operation,
        [Description("#FFFF00")]
        ScheduledStop,
        [Description("#5C0CF0")]
        UnscheduledStop,
        [Description("#FFF")]
        RemainingTime,
        [Description("#6D6DA3")]
        StoppedMachine,
    }
}
