using Application.Objects.Dto_s.Machine;
using Domain.Models;
using Domain.Models.Enums.Scheduling;
using Domain.Objects.Dto_s.Scheduling;

namespace Domain.Interfaces.Repositories
{
    public interface IMachineScheduleRepository : IBaseSqlRepository<MachineSchedule>
    {
        Task<MachineSchedule?> GetMachineScheduleByIdAndWeekDay(int userId, EWeekDay weekDay);
        Task<MachineScheduleDataDto?> GetFilteredMachineSchedule(int userId, DateTime startDateTime, EShiftType? shiftType);
        Task<IEnumerable<MachineScheduleDataDto>?> GetFilteredMachineSchedules(int userId, DateTime startDateTime, DateTime endDateTime, EShiftType? shiftType);
        Task<MachineSchedule?> GetMachineScheduleByUserIdAndWeekDay(int userId, DateTime startDateTime);
        Task<IEnumerable<ProductionTimeDto>?> GetWeekSchedulesByUserId(int userId);
    }
}
