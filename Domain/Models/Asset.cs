using Domain.Models.Bases;

namespace Domain.Models
{
    public class Asset : BaseSqlModel
    {
        public int AssetId { get; set; }
        public required string Name { get; set; }
        public required string Ip { get; set; }
        public int EggPackerQuantity { get; set; }
        public int DenesterQuantity { get; set; }
        public bool HasFeedback { get; set; }
        public string? Key { get; set; }
        public string? AuthToken { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int PlantId { get; set; }

        #region Relationships

        public required virtual Plant Plant { get; set; }
        public virtual ICollection<User>? Users { get; set; }
        public virtual ICollection<Product>? Products { get; set; }

        #endregion
    }
}

