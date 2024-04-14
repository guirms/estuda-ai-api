namespace Domain.Dto_s.Machine
{
    public record DefaultTimeDto
    {
        public required TimeSpan StartTime { get; set; }
        public required TimeSpan EndTime { get; set; }
    }
}
