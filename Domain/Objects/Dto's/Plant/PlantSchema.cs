using Domain.Models.Enums.Product;

namespace Domain.Objects.Requests.Plant
{
    public record PlantSchema
    {
        public int PlantId { get; set; }
        public int UserId { get; set; }
        public AssetSchema? AssetSchema { get; set; }
    }

    public record AssetSchema
    {
        public int AssetId { get; set; }
        public required string Ip { get; set; }
        public ICollection<ProductSchema>? ProductSchema { get; set; }
    }

    public record ProductSchema
    {
        public int ProductId { get; set; }
        public EProductType ProductType { get; set; }
        public required object PayloadSchema { get; set; }
        public ICollection<LayoutSchema>? LayoutSchema { get; set; }
    }

    public record LayoutSchema
    {
        public int LayoutId { get; set; }
        public required string Ip { get; set; }
    }
}