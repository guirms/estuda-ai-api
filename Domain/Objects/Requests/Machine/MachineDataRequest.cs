using Domain.Models.Enums.Machine;

namespace Domain.Objects.Requests.Machine
{
    public record MachineDataRequest
    {
        public int PlantId { get; set; }
        public int UserId { get; set; }
        public required AssetData AssetData { get; set; }
    }

    public record AssetData
    {
        public int AssetId { get; set; }
        public required ICollection<ProductData> ProductData { get; set; }
    }

    public record ProductData
    {
        public int ProductId { get; set; }
        public required ICollection<LayoutData> LayoutData { get; set; }
    }

    public record LayoutData
    {
        public int LayoutId { get; set; }
        public required PayloadData PayloadData { get; set; }
    }

    public record PayloadData
    {
        public DateTime CurrentDateTime { get; set; }
        public EMachineStatus? MachineStatus { get; set; }
        public EDevStatus? DevStatus { get; set; }
        public double? CurrentSpeed { get; set; }
        public double? ProgSpeed { get; set; }
        public Defects? Defects { get; set; }
        public Production? Production { get; set; }
        public WeightQuantity? WeightQuantity { get; set; }
        public ProductionQuantity? ProductionQuantity { get; set; }
        public CrackedQuantity? CrackedQuantity { get; set; }
    }

    public record Defects
    {
        public int Dirty { get; set; }
        public int Cracked { get; set; }
        public int Broken { get; set; }
        public int Leaked { get; set; }
    }

    public record Production
    {
        public int Total { get; set; }
        public int Bad { get; set; }
        public int White { get; set; }
        public int WhiteBad { get; set; }
        public double Fill { get; set; }
        public double Flow { get; set; }
    }

    public record WeightQuantity
    {
        public int? P1 { get; set; }
        public int? P2 { get; set; }
        public int? P3 { get; set; }
        public int? P4 { get; set; }
        public int? P5 { get; set; }
        public int? P6 { get; set; }
        public int P7 { get; set; }
    }

    public record ProductionQuantity
    {
        public int? P1 { get; set; }
        public int? P2 { get; set; }
        public int? P3 { get; set; }
        public int? P4 { get; set; }
        public int? P5 { get; set; }
        public int? P6 { get; set; }
        public int P7 { get; set; }
    }

    public record CrackedQuantity
    {
        public int? P1 { get; set; }
        public int? P2 { get; set; }
        public int? P3 { get; set; }
        public int? P4 { get; set; }
        public int? P5 { get; set; }
        public int? P6 { get; set; }
        public int P7 { get; set; }
    }
}