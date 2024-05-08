namespace Domain.Objects.Responses.Asset
{
    public record BoardResultsResponse
    {
        public int BoardId { get; set; }
        public required string Name { get; set; }
        public required DateTime ExamDateTime { get; set; }
        public required TimeSpan DailyStudyTime { get; set; }
    }
}

