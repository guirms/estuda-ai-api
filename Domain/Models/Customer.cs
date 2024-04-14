using Domain.Models.Bases;
using Domain.Models.Enums.Batch;

namespace Domain.Models
{
    public class Customer : BaseSqlModel
    {
        public int CustomerId { get; set; }
        public required string Name { get; set; }
        public required string Cnpj { get; set; }
        public EBatchStatus BatchStatus { get; set; }
        public int ProcessedBatches { get; set; }
    }
}

