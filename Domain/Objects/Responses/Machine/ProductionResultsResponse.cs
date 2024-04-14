using Domain.Models.Enums.Machine;
using Domain.Objects.Enums.Egg;
using Domain.Objects.Enums.Machine;

namespace Domain.Objects.Responses.Machine
{
    public record ProductionResultsResponse
    {
        public bool IsFiltered { get; set; }
        public string? EndDate { get; set; }
        public required IEnumerable<ProductionSchedule> ProductionSchedule { get; set; }
        public required PerformanceAndQuality PerformanceAndQuality { get; set; }
        public required Effectiveness Effectiveness { get; set; }
        public required IEnumerable<Runtime> Runtimes { get; set; }
        public required TotalData TotalData { get; set; }
    }

    public record ProductionSchedule
    {
        public required EMachineStatus MachineStatus { get; set; }
        public required EProductionScheduleStatus ProductionScheduleStatus { get; set; }
        public required string MachineStatusName { get; set; }
        public required string ProductionScheduleName { get; set; }
        public required string Color { get; set; }
        public required string StartTime { get; set; }
        public required string EndTime { get; set; }
        public required string Width { get; set; }
    }

    public record PerformanceAndQuality
    {
        public required string Availability { get; set; }
        public required string Performance { get; set; }
        public required string Quality { get; set; }
    }

    public record Runtime
    {
        public required ERuntimeStatus RuntimeStatus { get; set; }
        public required string RuntimeName { get; set; }
        public required string Hour { get; set; }
        public required string Percentage { get; set; }
    }

    public record Effectiveness
    {
        public required AverageProduction AverageProduction { get; set; }
        public required double Fill { get; set; }
        public required double Oee { get; set; }
    }

    public record AverageProduction
    {
        public double TotalSize { get; set; }
        public double Value { get; set; }
    }

    public record TotalData
    {
        public required string RealProduction { get; set; }
        public required string AverageRealSpeed { get; set; }
        public required string IdealSpeed { get; set; }
        public required string ScheduledStops { get; set; }
        public required string HoursAvailable { get; set; }
    }
}

