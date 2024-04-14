using Domain.Models.Bases;
using Domain.Models.Enums.Product;

namespace Domain.Models
{
    public class Product : BaseSqlModel
    {
        public int ProductId { get; set; }
        public required string Name { get; set; }
        public EProductType ProductType { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int AssetId { get; set; }

        #region Relationships

        public required virtual Asset Asset { get; set; }
        public virtual ICollection<Layout>? Layouts { get; set; }

        #endregion
    }
}

