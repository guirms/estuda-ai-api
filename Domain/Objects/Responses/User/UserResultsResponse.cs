namespace Domain.Objects.Responses.Asset
{
    public record UserResultsResponse
    {
        public int UserId { get; set; }
        public required string Name { get; set; }
        public required string Email { get; set; }
        public required string Document { get; set; }
    }
}
