using Domain.Objects.Requests.Plant;

namespace Domain.Objects.Responses.Asset
{
    public record AssetConfigResponse
    {
        public required string AssetAuthToken { get; set; }
        public required PlantSchema PlantSchema { get; set; }
    }
}

