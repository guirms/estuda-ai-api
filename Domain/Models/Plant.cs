using Domain.Models.Bases;

namespace Domain.Models
{
    public class Plant : BaseSqlModel
    {
        public int PlantId { get; set; }
        public required string Name { get; set; }
        public required string Cnpj { get; set; }
        public required string Address { get; set; }
        public required string ZipCode { get; set; }
        public required string Latitude { get; set; }
        public required string Longitude { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int UserId { get; set; }

        #region Relationships

        public virtual ICollection<Asset>? Assets { get; set; }
        public required virtual User User { get; set; }

        #endregion
    }
}

