using Domain.Dto_s.Machine;
using Domain.Models;
using Domain.Models.Enums.Scheduling;

namespace Domain.Interfaces.Repositories
{
    public interface IMachineOperationRepository : IBaseNoSqlRepository<MachineOperation>
    {
        Task<TimeSpan?> GetLastEndTimeByMachineScheduleId(int machineScheduleId);
        Task<IList<MachineOperationDto>?> GetFirstAndLastMachineOperationsByFilters(int machineScheduleId, int assetId, DateTime startDateTime, DateTime? endDateTime, EShiftType? shiftType);
        Task<IList<MachineOperationDto>?> GetLastestMachineOperationsByFilters(IEnumerable<int> machineScheduleIds, int assetId, DateTime startDateTime, DateTime endDateTime, EShiftType? shiftType);
        Task<IEnumerable<MachineOperationDataDto>?> GetMachineOperationsToFilter(IEnumerable<int> machineScheduleIds, int assetId, DateTime startDateTime, DateTime? endDateTime, EShiftType? shiftType, TimeSpan firstShiftDtoTime);
    }
}
