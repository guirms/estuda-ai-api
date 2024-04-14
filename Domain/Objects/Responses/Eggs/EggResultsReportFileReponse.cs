namespace Domain.Objects.Responses.Eggs
{
    public record EggResultsReportFileReponse
    {
        public EggResultsReportFileReponse(MemoryStream reportContent, string reportExtension)
        {
            ReportContent = reportContent;
            ReportExtension = reportExtension;
        }

        public MemoryStream ReportContent { get; set; }
        public string ReportExtension { get; set; }
    }
}
