namespace Domain.Objects.Responses.Machine
{
    public record ProductToTableResponse
    {
        public int ProductId { get; set; }
        public int AssetId { get; set; }
        public required string Name { get; set; }
        public int ProductType { get; set; }
    }
}

