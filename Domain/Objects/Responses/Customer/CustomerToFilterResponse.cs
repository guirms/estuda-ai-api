namespace Domain.Objects.Responses.Customer
{
    public record CustomerToFilterResponse
    {
        public int CustomerId { get; set; }
        public required string Name { get; set; }
    }
}
