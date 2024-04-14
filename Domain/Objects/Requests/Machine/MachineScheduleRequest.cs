using Domain.Models.Enums.Scheduling;
using FluentValidation;

namespace Domain.Objects.Requests.Machine
{
    public record MachineScheduleRequest
    {
        public EWeekDay WeekDay { get; set; }
        public IEnumerable<ScheduledStopRequest>? ScheduledStopsRequest { get; set; }
        public IEnumerable<ShiftRequest>? ShiftsRequest { get; set; }
        public required string InitialProductionTime { get; set; }
        public required string FinalProductionTime { get; set; }
    }

    public record ScheduledStopRequest
    {
        public required string Name { get; set; }
        public required string StartTime { get; set; }
        public required string EndTime { get; set; }
    }

    public record ShiftRequest
    {
        public int? ShiftId { get; set; }
        public required EShiftType Type { get; set; }
        public required string StartTime { get; set; }
        public required string EndTime { get; set; }
    }

    public class MachineScheduleRequestValidator : AbstractValidator<MachineScheduleRequest>
    {
        public MachineScheduleRequestValidator()
        {
            RuleFor(m => m)
                .Must(HaveValidTimeAndType);

            RuleForEach(m => m.ScheduledStopsRequest)
                .Must(HaveValidScheduledStop);

            RuleForEach(m => m.ShiftsRequest)
                .Must(BeValidShift);
        }

        private static bool HaveValidTimeAndType(MachineScheduleRequest machineScheduleRequest)
        {
            if (TimeSpan.Parse(machineScheduleRequest.FinalProductionTime) < TimeSpan.Parse(machineScheduleRequest.FinalProductionTime))
                throw new ValidationException("ErrorValidatingDate");

            if (machineScheduleRequest.ShiftsRequest != null && machineScheduleRequest.ShiftsRequest.Any())
            {
                var hasEqualShiftTypes = machineScheduleRequest.ShiftsRequest
                    .GroupBy(m => m.Type)
                    .Where(s => s.Count() > 1)
                    .SelectMany(s => s)
                    .Count() > 1;

                if (hasEqualShiftTypes)
                    throw new ValidationException("ErrorValidatingShift");
            }

            return true;
        }

        private static bool HaveValidScheduledStop(ScheduledStopRequest scheduledStop)
        {
            if (scheduledStop.Name.Length > 30)
                throw new ValidationException("InvalidScheduledStopName");

            if (TimeSpan.Parse(scheduledStop.EndTime) < TimeSpan.Parse(scheduledStop.StartTime))
                throw new ValidationException("ErrorValidatingDate");

            return true;
        }

        private static bool BeValidShift(ShiftRequest shift)
        {
            if (TimeSpan.Parse(shift.EndTime) < TimeSpan.Parse(shift.StartTime))
                throw new ValidationException("ErrorValidatingDate");

            return true;
        }
    }
}
