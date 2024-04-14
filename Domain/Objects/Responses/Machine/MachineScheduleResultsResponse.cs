using Domain.Models.Enums.Scheduling;
using Domain.Objects.Enums.Egg;

namespace Domain.Objects.Responses.Machine
{
    public record MachineScheduleResultsResponse
    {
        public EWeekDay WeekDay { get; set; }
        public required string InitialProductionTime { get; set; }
        public required string FinalProductionTime { get; set; }
        public required IEnumerable<double> TimeProduction { get; set; }
        public required IEnumerable<ProductionTime> ProductionTimes { get; set; }
        public required IEnumerable<double> WeekAvailability { get; set; }
        public required ICollection<double> ScheduledStops { get; set; }
    }

    public record ProductionTime
    {
        public required string StartTime { get; set; }
        public List<ProductionTimeContent>? Content { get; set; }
    }

    public record ProductionTimeContent
    {
        public int? ShiftId { get; set; }
        public required int TimeId { get; set; }
        public EOperationType Type { get; set; }
        public required string Name { get; set; }
        public required string StartDowntime { get; set; }
        public required string EndDowntime { get; set; }
        public required string Height { get; set; }
        public required string Top { get; set; }
    }
}
