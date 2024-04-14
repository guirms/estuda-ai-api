using Application.Objects.Dto_s.Machine;
using AutoMapper;
using Domain.Dto_s.Machine;
using Domain.Dto_s.Scheduling;
using Domain.Interfaces.Repositories;
using Domain.Models;
using Domain.Models.Enums.Scheduling;
using Domain.Objects.Dto_s.Egg;
using Domain.Objects.Dto_s.Scheduling;
using Domain.Utils.Helpers;
using Infra.Data.Context;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Infra.Data.Repositories
{

    public class MachineScheduleRepository(SqlContext context, IMapper mapper) : BaseSqlRepository<MachineSchedule>(context), IMachineScheduleRepository
    {
        public async Task<MachineSchedule?> GetMachineScheduleByIdAndWeekDay(int userId, EWeekDay weekDay)
        {
            return await _typedContext
                .Include(m => m.Shifts)
                .Include(m => m.ScheduledStops)
                .FirstOrDefaultAsync(m => m.UserId == userId &&
                    m.WeekDay == weekDay);
        }

        public async Task<MachineScheduleDataDto?> GetFilteredMachineSchedule(int userId, DateTime startDateTime, EShiftType? shiftType)
        {
            var queryMapper = GetMachineScheduleQueryMapper(shiftType);

            return queryMapper.Map<MachineScheduleDataDto>(await _typedContext
                    .AsNoTracking()
                    .Include(m => m.Shifts)
                    .Include(m => m.ScheduledStops)
                    .Where(m =>
                        m.UserId == userId &&
                        m.WeekDay == (EWeekDay)startDateTime.DayOfWeek)
                    .FirstOrDefaultAsync());
        }

        public async Task<IEnumerable<MachineScheduleDataDto>?> GetFilteredMachineSchedules(int userId, DateTime startDateTime, DateTime endDateTime, EShiftType? shiftType)
        {
            var queryProjector = GetMachineScheduleQueryProjector(shiftType);

            var diffDays = GetWeekdaysBetweenDates(startDateTime, endDateTime);

            return await queryProjector.ProjectTo<MachineScheduleDataDto>(_typedContext?
                    .AsNoTracking()
                    .Where(m =>
                        m.UserId == userId &&
                        diffDays.Contains(m.WeekDay))
                    .OrderBy(m => m.MachineScheduleId))
                    .ToListAsync();
        }

        public async Task<MachineSchedule?> GetMachineScheduleByUserIdAndWeekDay(int userId, DateTime startDateTime)
        {
            return await _typedContext
                .Include(m => m.Shifts)
                .FirstOrDefaultAsync(m =>
                    m.UserId == userId &&
                    m.WeekDay == (EWeekDay)startDateTime.DayOfWeek &&
                    m.InitialProductionTime <= startDateTime.TimeOfDay &&
                    m.FinalProductionTime >= startDateTime.TimeOfDay);
        }

        public async Task<IEnumerable<ProductionTimeDto>?> GetWeekSchedulesByUserId(int userId)
        {
            return await mapper.ProjectTo<ProductionTimeDto>(_typedContext
                .AsNoTracking()
                .Include(m => m.Shifts)
                .Include(m => m.ScheduledStops)
                .Where(m => m.UserId == userId))
                .ToListAsync();
        }

        private static IMapper GetMachineScheduleQueryMapper(EShiftType? shiftType)
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<MachineSchedule, MachineScheduleDataDto>()
                    .ForMember(m => m.FirstShiftDto, opts => opts.MapFrom(m => AddShiftCondition(true, m.Shifts, shiftType)))
                    .ForMember(m => m.LastShiftDto, opts => opts.MapFrom(m => AddShiftCondition(false, m.Shifts, shiftType)))
                    .ForMember(m => m.ScheduledStopsDto, opts => opts.MapFrom(m => m.ScheduledStops));

                cfg.CreateMap<MachineOperation, MachineOperationDto>()
                    .ForMember(m => m.EggQuantitiesDto, opts => opts.MapFrom(m => m.EggQuantities));

                cfg.CreateMap<EggQuantity, EggQuantityDto>();

                cfg.CreateMap<Shift, ShiftDto>();

                cfg.CreateMap<ScheduledStop, ScheduledStopDto>();
            });

            return configuration.CreateMapper();
        }

        private static IMapper GetMachineScheduleQueryProjector(EShiftType? shiftType)
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateProjection<MachineSchedule, MachineScheduleDataDto>()
                    .ForMember(m => m.FirstShiftDto, opts => opts.MapFrom(m => AddShiftCondition(true, m.Shifts, shiftType)))
                    .ForMember(m => m.LastShiftDto, opts => opts.MapFrom(m => AddShiftCondition(false, m.Shifts, shiftType)));

                cfg.CreateProjection<MachineOperation, MachineOperationDto>()
                    .ForMember(m => m.EggQuantitiesDto, opts => opts.MapFrom(m => m.EggQuantities));

                cfg.CreateProjection<EggQuantity, EggQuantityDto>();

                cfg.CreateProjection<Shift, ShiftDto>();

                cfg.CreateProjection<ScheduledStop, ScheduledStopDto>();
            });

            return configuration.CreateMapper();
        }

        private static IEnumerable<EWeekDay> GetWeekdaysBetweenDates(DateTime startDateTime, DateTime endDateTime)
        {
            var diffDays = new List<EWeekDay>();

            var dateDiff = endDateTime - startDateTime;

            if (dateDiff.Days == 0)
            {
                diffDays.Add((EWeekDay)startDateTime.DayOfWeek);
                return diffDays;
            }

            if (dateDiff.Days >= 7)
                return EnumHelper.GetEnumProperties<EWeekDay>();

            for (int i = 0; i <= dateDiff.Days; i++)
            {
                var currentDate = startDateTime.AddDays(i);

                diffDays.Add((EWeekDay)currentDate.DayOfWeek);
            }

            return diffDays;
        }

        private static Shift? AddShiftCondition(bool getFirst, ICollection<Shift>? shifts, EShiftType? shiftType)
        {
            if (shifts!.Count == 0)
                return null;

            if (shiftType.HasValue)
                return getFirst ? shifts!.FirstOrDefault(s => s.Type == shiftType.Value) : shifts!.LastOrDefault(s => s.Type == shiftType.Value);

            return getFirst ? shifts.FirstOrDefault() : shifts!.LastOrDefault();
        }
    }
}
