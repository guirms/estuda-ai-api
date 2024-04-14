namespace Domain.Objects.Requests.Machine
{
    public record MailDataRequest
    {
        public required string EmailToId { get; set; }
        public required string EmailToName { get; set; }
        public required string EmailSubject { get; set; }
        public required string EmailBody { get; set; }
    }
}