using Domain.Models.Enums.Scheduling;

namespace Domain.Dto_s.Scheduling
{
    public record ShiftDto
    {
        public EShiftType Type { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
    }
}
