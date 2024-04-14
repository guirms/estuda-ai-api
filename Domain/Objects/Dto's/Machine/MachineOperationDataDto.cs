using Domain.Objects.Dto_s.Egg;
using MongoDB.Bson;

namespace Domain.Dto_s.Machine
{
    public record MachineOperationDataDto
    {
        public ObjectId Id { get; set; }
        public TimeSpan EndTime { get; set; }
        public int TotalProduction { get; set; }
        public int TotalBad { get; set; }
        public int TotalWhite { get; set; }
        public int TotalCracked { get; set; }
        public int TotalDirty { get; set; }
        public int TotalLeaked { get; set; }
        public int TotalBroken { get; set; }
        public required IEnumerable<EggQuantityDto> EggQuantitiesDto { get; set; }
    }
}