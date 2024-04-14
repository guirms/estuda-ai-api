using Domain.Models.Enums.Machine;
using Domain.Objects.Dto_s.Egg;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.Dto_s.Machine
{
    [BsonIgnoreExtraElements]
    public record MachineOperationDto
    {
        public ObjectId Id { get; set; }
        public EMachineStatus MachineStatus { get; set; }
        public EDevStatus DevStatus { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public double? CurrentSpeed { get; set; }
        public double? ProgSpeed { get; set; }
        public double? Fill { get; set; }
        public double? Flow { get; set; }
        public int TotalProduction { get; set; }
        public int TotalBad { get; set; }
        public int TotalWhite { get; set; }
        public int TotalWhiteBad { get; set; }
        public int TotalCracked { get; set; }
        public int TotalDirty { get; set; }
        public int TotalLeaked { get; set; }
        public int TotalBroken { get; set; }
        public required IList<EggQuantityDto> EggQuantitiesDto { get; set; }
    }
}
