namespace Domain.Objects.Responses.Board
{
    public record CardResultsResponse
    {
        public IEnumerable<CardContentResponse>? ToDo { get; set; }
        public IEnumerable<CardContentResponse>? Doing { get; set; }
        public IEnumerable<CardContentResponse>? Done { get; set; }
    }

    public record CardContentResponse
    {
        public int CardId { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public double Order { get; set; }
        public required string StudyTime { get; set; }
    }
}