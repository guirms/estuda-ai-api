using Domain.Models.Bases;

namespace Domain.Models
{
    public class Layout : BaseSqlModel
    {
        public int LayoutId { get; set; }
        public required string Name { get; set; }
        public required string Ip { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int ProductId { get; set; }

        #region Relationships

        public required virtual Product Product { get; set; }

        #endregion
    }
}