using Domain.Objects.Responses.Asset;

namespace Domain.Objects.Responses.Machine
{
    public record PlantToFilterResponse
    {
        public int? SelectedPlantId { get; set; }
        public int? SelectedAssetId { get; set; }
        public IEnumerable<PlantToFilterData>? PlantData { get; set; }
        public IEnumerable<AssetToFilterResponse>? AssetData { get; set; }
    }

    public record PlantToFilterData
    {
        public int PlantId { get; set; }
        public required string Name { get; set; }
    }
}

