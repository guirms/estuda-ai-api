namespace Domain.Objects.Responses.Machine
{
    public record PlantToTableResponse
    {
        public int PlantId { get; set; }
        public int UserId { get; set; }
        public required string Name { get; set; }
        public required string Cnpj { get; set; }
        public required string Address { get; set; }
        public required string ZipCode { get; set; }
        public required string Latitude { get; set; }
        public required string Longitude { get; set; }
    }
}

