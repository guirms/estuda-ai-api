using Application.Interfaces;
using AutoMapper;
using Domain.Dto_s.Machine;
using Domain.Interfaces.Repositories;
using Domain.Models;
using Domain.Models.Enums.Scheduling;
using Domain.Objects.Enums.Egg;
using Domain.Objects.Requests.Machine;
using Domain.Objects.Responses.Machine;
using Domain.Utils.Helpers;
using Domain.Utils.Languages;
using System.ComponentModel.DataAnnotations;

namespace Domain.Services
{
    public class MachineScheduleService(IMachineScheduleRepository machineScheduleRepository, IMapper mapper) : IMachineScheduleService
    {
        public async Task<MachineScheduleResultsResponse> Get(EWeekDay weekDay)
        {
            var productionTimesDto = await machineScheduleRepository.GetWeekSchedulesByUserId(HttpContextHelper.GetUserId())
                ?? throw new InvalidOperationException("NoWeekScheduledFound");

            var productionTimeDto = productionTimesDto.FirstOrDefault(m => m.WeekDay == weekDay)
                ?? throw new InvalidOperationException("NoMachineScheduleFound");

            #region Necessary properties

            var totalMachineScheduledTime = productionTimeDto.FinalProductionTime.Subtract(productionTimeDto.InitialProductionTime).TotalMinutes;

            double totalMachineScheduledStopsTime, totalWeekMachineScheduleTime;
            totalMachineScheduledStopsTime = totalWeekMachineScheduleTime = 0;

            if (productionTimeDto.ScheduledStops.Count != 0)
                totalMachineScheduledStopsTime = productionTimeDto.ScheduledStops.Last().EndTime
                    .Subtract(productionTimeDto.ScheduledStops.First().StartTime).TotalMinutes;

            #endregion

            #region Scheduled stops

            var scheduledStops = new List<double>(7);

            productionTimesDto = productionTimesDto.OrderBy(ms => ms.WeekDay == 0 ? 7 : (int)ms.WeekDay);
            foreach (var schedule in productionTimesDto)
            {
                if (schedule.ScheduledStops.Count != 0)
                    scheduledStops.Add(GetSumScheduledStops(schedule.ScheduledStops));
                else
                    scheduledStops.Add(0);
            }

            #endregion

            #region Time production

            IEnumerable<double> timeProduction = new List<double>(2) { totalMachineScheduledStopsTime, totalMachineScheduledTime };

            #endregion

            #region Production times

            var productionTimes = MapToProductionTimes(productionTimeDto.InitialProductionTime, productionTimeDto.FinalProductionTime, productionTimeDto.Shifts, productionTimeDto.ScheduledStops);

            #endregion

            #region Week avalaibility

            var totalWeekOperationTime = productionTimesDto.ToList().Sum(w => w.FinalProductionTime.Subtract(w.InitialProductionTime).TotalMinutes);
            totalWeekMachineScheduleTime = scheduledStops.Sum();
            totalWeekMachineScheduleTime = 100 - Math.Round(totalWeekMachineScheduleTime * 100 / totalWeekOperationTime, 2);

            IEnumerable<double> weekAvailability = new List<double>(2) { totalWeekMachineScheduleTime };

            #endregion

            return new MachineScheduleResultsResponse
            {
                WeekDay = productionTimeDto.WeekDay,
                InitialProductionTime = productionTimeDto.InitialProductionTime.ToString(@"hh\:mm"),
                FinalProductionTime = productionTimeDto.FinalProductionTime.ToString(@"hh\:mm"),
                TimeProduction = timeProduction,
                ProductionTimes = productionTimes,
                WeekAvailability = weekAvailability,
                ScheduledStops = scheduledStops,
            };
        }

        public async Task Update(MachineScheduleRequest machineScheduleRequest)
        {
            var machineSchedule = await machineScheduleRepository.GetMachineScheduleByIdAndWeekDay(HttpContextHelper.GetUserId(), machineScheduleRequest.WeekDay)
                ?? throw new InvalidOperationException("NoMachineScheduleFound");

            if (machineSchedule.ScheduledStops != null && machineSchedule.ScheduledStops.Count != 0)
                machineSchedule.ScheduledStops = new List<ScheduledStop>();

            if (machineSchedule.Shifts != null && machineSchedule.Shifts.Count != 0)
                machineSchedule.Shifts = new List<Shift>();

            var currentDateTime = DateTime.Now;

            if (machineScheduleRequest.ShiftsRequest != null)
            {
                if (HasTimesInConflict([.. mapper.ProjectTo<DefaultTimeDto>(machineScheduleRequest.ShiftsRequest.AsQueryable())]))
                    throw new ValidationException("ShiftConflict");

                machineSchedule.Shifts = mapper.ProjectTo<Shift>(machineScheduleRequest.ShiftsRequest.AsQueryable()).ToList();
                machineSchedule.UpdatedAt = currentDateTime;
            }

            if (machineScheduleRequest.ScheduledStopsRequest != null)
            {
                if (HasTimesInConflict([.. mapper.ProjectTo<DefaultTimeDto>(machineScheduleRequest.ScheduledStopsRequest.AsQueryable())]))
                    throw new ValidationException("ScheduledStopConflict");

                machineSchedule.ScheduledStops = mapper.ProjectTo<ScheduledStop>(machineScheduleRequest.ScheduledStopsRequest.AsQueryable()).ToList();
                machineSchedule.UpdatedAt = currentDateTime;
            }

            if (machineSchedule.InitialProductionTime != TimeSpan.Parse(machineScheduleRequest.InitialProductionTime))
            {
                machineSchedule.InitialProductionTime = TimeSpan.Parse(machineScheduleRequest.InitialProductionTime);
                machineSchedule.UpdatedAt = currentDateTime;
            }

            if (machineSchedule.FinalProductionTime != TimeSpan.Parse(machineScheduleRequest.FinalProductionTime))
            {
                machineSchedule.FinalProductionTime = TimeSpan.Parse(machineScheduleRequest.FinalProductionTime);
                machineSchedule.UpdatedAt = currentDateTime;
            }

            await machineScheduleRepository.Update(machineSchedule);
        }

        private static double GetSumScheduledStops(ICollection<ScheduledStop> scheduledStops)
        {
            double totalScheduledStops = 0;

            foreach (var scheduledStop in scheduledStops)
            {
                totalScheduledStops += scheduledStop.EndTime.Subtract(scheduledStop.StartTime).TotalMinutes;
            }

            return totalScheduledStops;
        }

        private static List<ProductionTime> MapToProductionTimes(TimeSpan initialProductionTime, TimeSpan finalProductionTime, ICollection<Shift>? shifts, ICollection<ScheduledStop>? scheduledStops)
        {
            var id = 1;
            var productionTimes = new List<ProductionTime>();
            var timeList = GenerateTimeList(initialProductionTime, finalProductionTime);

            foreach (var time in timeList)
            {
                productionTimes.Add(new ProductionTime
                {
                    StartTime = time,
                });
            }

            if (shifts != null || scheduledStops != null)
                for (var i = 0; i < timeList.Count; i++)
                {
                    if (shifts != null)
                        foreach (var shift in shifts)
                        {
                            if (shift.StartTime.ToString()[..2] == timeList[i][..2])
                            {
                                var totalShiftTime = shift.EndTime.Subtract(shift.StartTime).TotalMinutes;
                                var totalHeight = $"{(totalShiftTime * 0.55916).ToString().Replace(',', '.')}px";

                                var totalTop = $"{shift.StartTime.Minutes * 0.5533}px";

                                if (productionTimes[i].Content == null)
                                    productionTimes[i].Content = [];

                                productionTimes[i].Content?.Add(new ProductionTimeContent
                                {
                                    ShiftId = shift.ShiftId,
                                    TimeId = id++,
                                    Type = EOperationType.Shift,
                                    Name = Translator.Translate("Shift" + shift.Type.GetName()),
                                    StartDowntime = shift.StartTime.ToString(@"hh\:mm"),
                                    EndDowntime = shift.EndTime.ToString(@"hh\:mm"),
                                    Height = totalHeight,
                                    Top = totalTop.ToString().Replace(',', '.'),
                                });
                            }
                        }

                    if (scheduledStops != null)
                        foreach (var scheduledStop in scheduledStops)
                        {
                            if (scheduledStop.StartTime.ToString()[..2] == timeList[i][..2])
                            {
                                var totalShiftTime = scheduledStop.EndTime.Subtract(scheduledStop.StartTime).TotalMinutes;
                                var totalHeight = $"{(totalShiftTime * 0.55916).ToString().Replace(',', '.')}px";
                                var totalTop = $"{scheduledStop.StartTime.Minutes * 0.5533}px";

                                if (productionTimes[i].Content == null)
                                    productionTimes[i].Content = [];

                                productionTimes[i].Content?.Add(new ProductionTimeContent
                                {
                                    TimeId = id++,
                                    Type = EOperationType.ScheduledStop,
                                    Name = scheduledStop.Name,
                                    StartDowntime = scheduledStop.StartTime.ToString(@"hh\:mm"),
                                    EndDowntime = scheduledStop.EndTime.ToString(@"hh\:mm"),
                                    Height = totalHeight,
                                    Top = totalTop.ToString().Replace(',', '.'),
                                });
                            }
                        }
                }

            return productionTimes;
        }

        private static List<string> GenerateTimeList(TimeSpan startTime, TimeSpan endTime)
        {
            var timeList = new List<string>
            {
                startTime.ToString(@"hh\:mm")
            };

            var interval = TimeSpan.FromHours(1);
            var currentTime = RoundTime(startTime, interval);

            while (currentTime.Hours < endTime.Hours)
            {
                currentTime = currentTime.Add(interval);
                timeList.Add(currentTime.ToString(@"hh\:mm"));
            }

            return timeList;
        }

        private static TimeSpan RoundTime(TimeSpan time, TimeSpan interval)
        {
            var delta = (time.Ticks + interval.Ticks / 2) / interval.Ticks;

            return new TimeSpan(delta * interval.Ticks);
        }

        public static bool HasTimesInConflict(List<DefaultTimeDto> defaultTimesDto)
        {
            for (int i = 0; i < defaultTimesDto.Count; i++)
            {
                for (int j = i + 1; j < defaultTimesDto.Count; j++)
                {
                    if (IsScheduledStopsSuperimposed(defaultTimesDto[i], defaultTimesDto[j]))
                        return true;
                }
            }

            return false;
        }

        private static bool IsScheduledStopsSuperimposed(DefaultTimeDto firstTime, DefaultTimeDto secondTime)
        {
            var firstStartTime = firstTime.StartTime;
            var firstEndTime = firstTime.EndTime;
            var secondStartTime = secondTime.StartTime;
            var secondEndTime = secondTime.EndTime;

            return firstStartTime < secondEndTime && firstEndTime > secondStartTime;
        }
    }
}
