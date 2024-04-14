namespace Domain.Objects.Requests.Batch
{
    public record BatchRequest
    {
        public int CustomerId { get; set; }
        public required string Name { get; set; }
        public required string Cnpj { get; set; }
        public required string AuthToken { get; set; }
    }
}
