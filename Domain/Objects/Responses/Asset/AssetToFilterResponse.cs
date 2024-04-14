namespace Domain.Objects.Responses.Asset
{
    public record AssetToFilterResponse
    {
        public int AssetId { get; set; }
        public required string Name { get; set; }
    }
}
