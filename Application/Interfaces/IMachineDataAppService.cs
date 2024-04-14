using Domain.Objects.Requests.Report;
using Domain.Objects.Responses.Eggs;

namespace Application.Interfaces
{
    public interface IMachineDataAppService
    {
        EggResultsReportFileReponse GenerateEggResultsReportFile(EggResultsReportFileRequest eggResultsReportFileRequest);
    }
}
