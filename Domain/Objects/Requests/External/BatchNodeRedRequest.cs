namespace Infra.CrossCutting.Externals.Objects.Requests
{
    public record BatchNodeRedRequest
    {
        public bool Result { get; set; }
        public required string InsertedDateTime { get; set; }
        public required string CustomerId { get; set; }
    }
}
