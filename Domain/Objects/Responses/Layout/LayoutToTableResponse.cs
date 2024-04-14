namespace Domain.Objects.Responses.Machine
{
    public record LayoutToTableResponse
    {
        public int LayoutId { get; set; }
        public int ProductId { get; set; }
        public required string Name { get; set; }
        public required string Ip { get; set; }
    }
}

