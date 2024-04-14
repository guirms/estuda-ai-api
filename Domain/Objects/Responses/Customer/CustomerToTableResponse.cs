using Domain.Models.Enums.Batch;

namespace Domain.Objects.Responses.Customer
{
    public record CustomerToTableResponse
    {
        public string? CurrentBatchRunning { get; set; }
        public IEnumerable<CustomerTableData>? CustomerTableData { get; set; }
        public bool IsBatchDisabled { get; set; }
    }

    public record CustomerTableData
    {
        public int CustomerId { get; set; }
        public bool IsSelected { get; set; }
        public required string Name { get; set; }
        public required string Cnpj { get; set; }
        public EBatchStatus BatchStatusValue { get; set; }
        public required string BatchStatusText { get; set; }
        public int ProcessedBatches { get; set; }
    }
}
