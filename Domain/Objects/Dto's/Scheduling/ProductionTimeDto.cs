using Domain.Models;
using Domain.Models.Enums.Scheduling;

namespace Domain.Objects.Dto_s.Scheduling
{
    public record ProductionTimeDto
    {
        public EWeekDay WeekDay { get; set; }
        public TimeSpan InitialProductionTime { get; set; }
        public TimeSpan FinalProductionTime { get; set; }

        public required ICollection<Shift> Shifts { get; set; }
        public required ICollection<ScheduledStop> ScheduledStops { get; set; }
    }
}
