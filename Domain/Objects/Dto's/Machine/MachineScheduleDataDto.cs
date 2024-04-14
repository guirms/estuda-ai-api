using Domain.Dto_s.Scheduling;
using FluentValidation;

namespace Application.Objects.Dto_s.Machine
{
    public record MachineScheduleDataDto
    {
        public int MachineScheduleId { get; set; }
        public required ICollection<ScheduledStopDto> ScheduledStopsDto { get; set; }
        public required ShiftDto FirstShiftDto { get; set; }
        public required ShiftDto LastShiftDto { get; set; }

        public bool HasShifts() => FirstShiftDto != null && LastShiftDto != null;
    }

    public class MachineScheduleDataDtoValidator : AbstractValidator<MachineScheduleDataDto>
    {
        public MachineScheduleDataDtoValidator()
        {
            RuleFor(m => m)
                .Must(HaveValidShifts);
        }

        private static bool HaveValidShifts(MachineScheduleDataDto machineScheduleDto)
        {
            if (!machineScheduleDto.HasShifts())
                throw new ValidationException("NoShiftFound");

            return true;
        }
    }

    public class MachineSchedulesDataDtoValidator : AbstractValidator<IEnumerable<MachineScheduleDataDto>>
    {
        public MachineSchedulesDataDtoValidator()
        {
            RuleFor(m => m)
                .Must(HaveAnyShift);
        }

        private static bool HaveAnyShift(IEnumerable<MachineScheduleDataDto> machineSchedulesDto)
        {
            var hasShift = false;

            foreach (var machineSchedule in machineSchedulesDto)
            {
                if (machineSchedule.HasShifts())
                    hasShift = true;
            }

            if (!hasShift)
                throw new ValidationException("NoShiftFound");

            return hasShift;
        }
    }
}
