using Domain.Models.Enums.Scheduling;
using Domain.Objects.Requests.Machine;
using Domain.Objects.Responses.Machine;

namespace Application.Interfaces
{
    public interface IMachineDataService
    {
        Task Save(MachineDataRequest machineDataRequest);
        Task<ProductionResultsResponse> GetProductionResults(DateTime startDateTime, bool isOptoClass, bool isFeeback, DateTime? endDateTime, EShiftType? shiftType);
        Task<EggResultsResponse> GetEggResults(DateTime startDateTime, DateTime endDateTime, bool isOptoClass, EShiftType? shiftType, int[]? customerIds);
        Task<IEnumerable<GeneralProductionData>> GetDefaultEggCategories(bool isOptoClass);
    }
}
