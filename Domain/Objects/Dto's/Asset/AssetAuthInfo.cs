namespace Domain.Objects.Dto_s.Asset
{
    public record AssetAuthInfo
    {
        public required string Key { get; set; }
        public required string AuthToken { get; set; }
    }
}
