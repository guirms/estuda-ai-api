namespace Domain.Objects.Responses.Asset
{
    public record AssetToTableResponse
    {
        public int AssetId { get; set; }
        public int PlantId { get; set; }
        public required string Name { get; set; }
        public required string Ip { get; set; }
        public int ModelType { get; set; }
        public int VisionSystemType { get; set; }
        public int EggWasherType { get; set; }
        public int DryerType { get; set; }
        public int EggPackerQuantity { get; set; }
        public int DenesterQuantity { get; set; }
        public int HasFeedback { get; set; }
    }
}
