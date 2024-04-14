using Bogus;
using Domain.Dto_s.Scheduling;

namespace Test.Fakers
{
    internal static class MachineScheduleDataDtoFaker
    {
        //internal static MachineScheduleDataDto GenerateEmptyMachineScheduleDataDto()
        //{
        //    return new Faker<MachineScheduleDataDto>()
        //        .RuleFor(m => m.MachineOperationsDto, f => [])
        //        .RuleFor(m => m.ShiftsDto, f => [])
        //        .RuleFor(m => m.ScheduledStopsDto, f => [])
        //        .Generate();
        //}

        //internal static IQueryable<MachineScheduleDataDto> GenerateEmptyMachineSchedulesDataDto(int count = 1)
        //{
        //    return new Faker<MachineScheduleDataDto>()
        //        .RuleFor(m => m.MachineOperationsDto, f => [])
        //        .RuleFor(m => m.ShiftsDto, f => [])
        //        .RuleFor(m => m.ScheduledStopsDto, f => [])
        //        .Generate(count).AsQueryable();
        //}

        internal static ShiftDto GenerateShiftWithCurrentTimeShorterThanShiftTime()
        {
            var timeSpanShorterThanCurrentTime = DateTime.Now.AddHours(-1).TimeOfDay;

            return new Faker<ShiftDto>()
                .RuleFor(s => s.StartTime, f => timeSpanShorterThanCurrentTime)
                .RuleFor(s => s.EndTime, f => timeSpanShorterThanCurrentTime)
                .Generate();
        }

        internal static ShiftDto GenerateShiftWithCurrentTimeGreaterThanShiftTime()
        {
            var timeSpanGreaterThanCurrentTime = DateTime.Now.AddHours(1).TimeOfDay;

            return new Faker<ShiftDto>()
                .RuleFor(s => s.StartTime, f => timeSpanGreaterThanCurrentTime)
                .RuleFor(s => s.EndTime, f => timeSpanGreaterThanCurrentTime)
                .Generate();
        }
    }
}
