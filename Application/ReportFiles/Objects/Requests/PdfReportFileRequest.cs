namespace Application.Reports.Objects.Requests
{
    public record PdfReportFileRequest
    {
        public required string StartDate { get; set; }
        public required string EndDate { get; set; }
    }
}
