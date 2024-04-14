namespace Domain.Objects.Responses.Machine
{
    public record EggResultsResponse
    {
        public bool IsFiltered { get; set; }
        public required TotalEggsData TotalEggsData { get; set; }
        public required IEnumerable<GeneralProductionData> GeneralProductionData { get; set; }
        public required IEnumerable<VisionSystemStatistics> VisionSystemStatistics { get; set; }
        public required IEnumerable<BoxesQuantity> BoxesQuantity { get; set; }
    }

    public record TotalEggsData
    {
        public required string EggsWeight { get; set; }
        public required string EggsQuantity { get; set; }
        public required string BoxesQuantity { get; set; }
    }

    public record GeneralProductionData
    {
        public required string EggName { get; set; }
        public required string EggWeight { get; set; }
        public required string EggQuantity { get; set; }
        public required string BoxQuantity { get; set; }
    }

    public record VisionSystemStatistics
    {
        public required string EggName { get; set; }
        public double EggPercentage { get; set; }
    }

    public record BoxesQuantity
    {
        public required double BoxQuantity { get; set; }
        public required string EggName { get; set; }
    }
}


