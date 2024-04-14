using Domain.Objects.Enums.Report;
using Domain.Objects.Responses.Machine;

namespace Domain.Objects.Requests.Report
{
    public record EggResultsReportFileRequest
    {
        public EReportType ReportType { get; set; }
        public required string StartDate { get; set; }
        public required string EndDate { get; set; }
        public required TotalEggsData TotalEggsData { get; set; }
        public required IEnumerable<GeneralProductionData> GeneralProductionData { get; set; }
        public required IEnumerable<VisionSystemStatistics> VisionSystemStatistics { get; set; }
    }
}
