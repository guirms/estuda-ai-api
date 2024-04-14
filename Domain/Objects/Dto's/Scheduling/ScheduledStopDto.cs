namespace Domain.Dto_s.Scheduling
{
    public record ScheduledStopDto
    {
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
    }
}
