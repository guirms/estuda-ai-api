using Domain.Models.Enums.Egg;

namespace Domain.Objects.Dto_s.Egg
{
    public partial record EggQuantityDto
    {
        public EEggQuantityType Type { get; set; }
        public int P1 { get; set; }
        public int P2 { get; set; }
        public int P3 { get; set; }
        public int P4 { get; set; }
        public int P5 { get; set; }
        public int P6 { get; set; }
        public int P7 { get; set; }
    }

    public partial record EggQuantityDto
    {
        public int SumEggQuantityProperties() => P1 + P2 + P3 + P4 + P5 + P6 + P7;
        public bool HasValueBelowZero() => P1 < 0 || P2 < 0 || P3 < 0 || P4 < 0 || P5 < 0 || P6 < 0 || P7 < 0;
    }
}
