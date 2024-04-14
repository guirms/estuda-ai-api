using Domain.Models.Enums.Scheduling;
using Domain.Objects.Requests.Machine;
using Domain.Objects.Responses.Machine;

namespace Application.Interfaces
{
    public interface IMachineScheduleService
    {
        Task<MachineScheduleResultsResponse> Get(EWeekDay weekDay);
        Task Update(MachineScheduleRequest machineScheduleRequest);
    }
}
