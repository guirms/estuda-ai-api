namespace Domain.Objects.Responses.Batch
{
    public record BatchInfoResponse
    {
        public int CustomerId { get; set; }
        public required string Name { get; set; }
        public required string Cnpj { get; set; }
    }
}
