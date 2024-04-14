using Application.Interfaces;
using Application.ReportFiles.Utils.Constants;
using Application.Reports.ReportFiles.EggResultsReport;
using Domain.Objects.Enums.Report;
using Domain.Objects.Requests.Report;
using Domain.Objects.Responses.Eggs;

namespace Application.AppServices
{
    public class MachineDataAppService(EggResultsReportFile eggResultsReport) : IMachineDataAppService
    {
        public EggResultsReportFileReponse GenerateEggResultsReportFile(EggResultsReportFileRequest eggResultsReportFileRequest)
        {
            if (eggResultsReportFileRequest.ReportType == EReportType.Pdf)
            {
                var eggResultsPdf = eggResultsReport.GetPdfReport(eggResultsReportFileRequest);

                if (eggResultsPdf != null)
                    return new EggResultsReportFileReponse(eggResultsPdf, ReportFileConfig.PdfExtension);
            }
            else if (eggResultsReportFileRequest.ReportType == EReportType.Excel)
            {
                var eggResultsExcel = eggResultsReport.GetExcelReport(eggResultsReportFileRequest);

                if (eggResultsExcel != null)
                    return new EggResultsReportFileReponse(eggResultsExcel, ReportFileConfig.ExcelExtension);
            }

            throw new InvalidOperationException("ErrorGeneratingReportFile");
        }
    }
}


