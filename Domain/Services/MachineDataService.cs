using Application.Interfaces;
using Application.Objects.Dto_s.Machine;
using AutoMapper;
using Domain.Dto_s.Machine;
using Domain.Dto_s.Scheduling;
using Domain.Interfaces.Repositories;
using Domain.Models;
using Domain.Models.Enums.Egg;
using Domain.Models.Enums.Machine;
using Domain.Models.Enums.Scheduling;
using Domain.Objects.Dto_s.Egg;
using Domain.Objects.Dto_s.Machine;
using Domain.Objects.Enums.Egg;
using Domain.Objects.Enums.Machine;
using Domain.Objects.Requests.Machine;
using Domain.Objects.Responses.Machine;
using Domain.Utils.Helpers;
using Domain.Utils.Languages;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace Domain.Services
{
    public class MachineDataService(IMachineScheduleRepository machineScheduleRepository, IMachineOperationRepository machineOperationRepository, IEggCategoryRepository eggCategoryRepository, ICustomerRepository customerRepository, IPlantRepository plantRepository, IAssetRepository assetRepository, IUserRepository userRepository, IValidator<IEnumerable<MachineScheduleDataDto>> machineSchedulesDataDtoValidator, IValidator<MachineScheduleDataDto> machineScheduleDataDtoValidator, IConfiguration configuration, IMapper mapper) : IMachineDataService
    {
        public async Task Save(MachineDataRequest machineDataRequest)
        {
            if (!await plantRepository.HasPlantById(machineDataRequest.PlantId))
                throw new InvalidOperationException("PlantNotFound");

            var currentDateTime = DateTime.Now;

            var machineSchedule = await machineScheduleRepository.GetMachineScheduleByUserIdAndWeekDay(machineDataRequest.UserId, currentDateTime)
                    ?? throw new InvalidOperationException("NoMachineScheduleFound");

            var currentTime = currentDateTime.TimeOfDay;
            var currentShift = machineSchedule.Shifts?
                .FirstOrDefault(s => s.StartTime <= currentTime && s.EndTime >= currentTime);

            var lastEndTime = await machineOperationRepository.GetLastEndTimeByMachineScheduleId(machineSchedule.MachineScheduleId);

            var machineOperation = new MachineOperation
            {
                AssetId = machineDataRequest.AssetData.AssetId,
                InsertedAt = currentDateTime,
                StartTime = lastEndTime ?? currentDateTime.TimeOfDay,
                EndTime = currentDateTime.TimeOfDay,
                ShiftType = currentShift?.Type,
                MachineScheduleId = machineSchedule.MachineScheduleId,
                CustomerId = await customerRepository.GetStartedCustomerIdById(),
                EggQuantities = []
            };

            var assetData = machineDataRequest.AssetData;

            if (!await assetRepository.HasAssetById(assetData.AssetId))
                throw new InvalidOperationException("AssetNotFound");

            foreach (var product in assetData.ProductData)
            {
                foreach (var layout in product.LayoutData)
                {
                    var payload = layout.PayloadData;

                    if (payload.MachineStatus != null)
                        machineOperation.MachineStatus = payload.MachineStatus.Value;

                    if (payload.DevStatus != null)
                        machineOperation.DevStatus = payload.DevStatus.Value;

                    if (payload.CurrentSpeed != null)
                        machineOperation.CurrentSpeed = payload.CurrentSpeed.Value;

                    if (payload.ProgSpeed != null)
                        machineOperation.ProgSpeed = payload.ProgSpeed.Value;

                    var defects = payload.Defects;
                    if (defects != null)
                    {
                        machineOperation.TotalDirty = defects.Dirty;
                        machineOperation.TotalCracked = defects.Cracked;
                        machineOperation.TotalBroken = defects.Broken;
                        machineOperation.TotalLeaked = defects.Leaked;
                    }

                    var production = payload.Production;
                    if (production != null)
                    {
                        machineOperation.TotalProduction = production.Total;
                        machineOperation.TotalBad = production.Bad;
                        machineOperation.TotalWhite = production.White;
                        machineOperation.TotalWhiteBad = production.WhiteBad;
                        machineOperation.Fill = production.Fill;
                        machineOperation.Flow = production.Flow;
                    }

                    var weightQuantity = payload.WeightQuantity;
                    var productionQuantity = payload.ProductionQuantity;
                    var crackedQuantity = payload.CrackedQuantity;
                    if (weightQuantity != null && productionQuantity != null && crackedQuantity != null)
                    {
                        var eggQuantities = new List<EggQuantity>(3)
                        {
                            mapper.Map<EggQuantity>(weightQuantity),
                            mapper.Map<EggQuantity>(productionQuantity),
                            mapper.Map<EggQuantity>(crackedQuantity)
                        };

                        machineOperation.EggQuantities = eggQuantities;
                    }
                }
            }

            await machineOperationRepository.Save(machineOperation);
        }

        public async Task<ProductionResultsResponse> GetProductionResults(DateTime startDateTime, bool isOptoClass, bool isFeeback, DateTime? endDateTime, EShiftType? shiftType)
        {
            var userId = HttpContextHelper.GetUserId();
            var assetId = await userRepository.GetAssetIdById(userId)
                ?? throw new InvalidOperationException("AssetNotFound");

            var machineScheduleDto = await machineScheduleRepository
                .GetFilteredMachineSchedule(userId, startDateTime, shiftType)
                    ?? throw new InvalidOperationException("NoMachineScheduleFound");

            machineScheduleDataDtoValidator.Validate(machineScheduleDto);

            var machineOperationsDto = await machineOperationRepository
                .GetFirstAndLastMachineOperationsByFilters(machineScheduleDto.MachineScheduleId, assetId, startDateTime, endDateTime, shiftType)
                ?? throw new ValidationException("NoMachineOperationFound");

            if (!machineOperationsDto.Any())
                throw new ValidationException("NoMachineOperationFound");

            #region Necessary properties

            var firstMachineOperation = machineOperationsDto.First();
            var lastMachineOperation = machineOperationsDto.Last();
            var scheduledStopsDto = machineScheduleDto.ScheduledStopsDto;

            var firstTimeToFilter = firstMachineOperation.StartTime;
            var lastTimeToFilter = lastMachineOperation.EndTime;

            var firstShiftDtoTime = machineScheduleDto.FirstShiftDto.StartTime;
            var lastShiftDtoTime = machineScheduleDto.LastShiftDto.EndTime;

            if (shiftType.HasValue)
            {
                firstTimeToFilter = firstShiftDtoTime;
                lastTimeToFilter = lastShiftDtoTime;

                if (!endDateTime.HasValue)
                {
                    startDateTime = new DateTime(startDateTime.Year, startDateTime.Month, startDateTime.Day,
                            firstShiftDtoTime.Hours, firstShiftDtoTime.Minutes, firstShiftDtoTime.Seconds);
                    endDateTime = new DateTime(startDateTime.Year, startDateTime.Month, startDateTime.Day,
                                                lastShiftDtoTime.Hours, lastShiftDtoTime.Minutes, lastShiftDtoTime.Seconds);
                }
            }
            if (endDateTime.HasValue)
            {
                firstTimeToFilter = startDateTime.TimeOfDay;
                lastTimeToFilter = endDateTime.Value.TimeOfDay;
            }

            if (lastMachineOperation.EndTime > lastTimeToFilter)
                lastMachineOperation.EndTime = lastTimeToFilter;

            if (firstMachineOperation.StartTime < firstTimeToFilter)
                firstMachineOperation.StartTime = firstTimeToFilter;

            var isFiltered = endDateTime.HasValue || shiftType.HasValue;

            IEnumerable<EggQuantityDto>? eggsQuantityDto = null;

            if (machineOperationsDto.Any(r => r.EggQuantitiesDto != null))
                eggsQuantityDto = machineOperationsDto.SelectMany(r => r.EggQuantitiesDto);

            var productionQuantity = (eggsQuantityDto?
                    .FirstOrDefault(e => e.Type == EEggQuantityType.Production)?
                    .SumEggQuantityProperties()) ?? 0;

            var totalProductionMagna = productionQuantity;

            int dirtyQuantity, leakedQuantity, crackedOptoQuantity, brokenQuantity;
            dirtyQuantity = leakedQuantity = crackedOptoQuantity = brokenQuantity = 0;
            double fill = 0;

            if (isOptoClass)
            {
                var prodQuantityOpto = lastMachineOperation.TotalProduction;
                productionQuantity = prodQuantityOpto > productionQuantity ? prodQuantityOpto : productionQuantity;

                dirtyQuantity = lastMachineOperation.TotalDirty;
                leakedQuantity = lastMachineOperation.TotalLeaked;
                crackedOptoQuantity = lastMachineOperation.TotalCracked;
                brokenQuantity = lastMachineOperation.TotalBroken;
                fill = lastMachineOperation.Fill ?? 0;
            }

            var crackedMagnaQuantity = (eggsQuantityDto?
                .FirstOrDefault(e => e.Type == EEggQuantityType.Cracked)?
                .SumEggQuantityProperties()) ?? 0;

            var progSpeed = lastMachineOperation?.ProgSpeed;
            var currentSpeed = lastMachineOperation?.CurrentSpeed;

            var totalOperationTime = GetSumOperationTime(machineOperationsDto);
            var initialSearchTime = startDateTime.TimeOfDay;
            var finalSearchTime = endDateTime.HasValue ? endDateTime.Value.TimeOfDay : TimeSpan.Zero;

            if (!endDateTime.HasValue)
            {
                initialSearchTime = firstShiftDtoTime;
                finalSearchTime = lastShiftDtoTime;
            }

            var totalSearchTime = finalSearchTime.Subtract(initialSearchTime);

            TimeSpan totalScheduledDowntime = new(0);

            var firstScheduledDowntimeTime = scheduledStopsDto.FirstOrDefault()?.StartTime ?? TimeSpan.Zero;
            var lastScheduledDowntimeTime = scheduledStopsDto.LastOrDefault()?.EndTime ?? TimeSpan.Zero;

            if (scheduledStopsDto.Count != 0)
                foreach (var scheduledStopDto in scheduledStopsDto)
                {
                    if (scheduledStopDto.StartTime <= firstTimeToFilter && scheduledStopDto.EndTime > firstTimeToFilter)
                    {
                        if (scheduledStopDto.EndTime <= lastTimeToFilter)
                        {
                            var timeToAdd = scheduledStopDto.StartTime >= firstTimeToFilter ? scheduledStopDto.EndTime.Subtract(scheduledStopDto.StartTime) : scheduledStopDto.EndTime.Subtract(firstTimeToFilter);
                            totalScheduledDowntime.Add(timeToAdd);
                        }
                        else
                        {
                            scheduledStopDto.StartTime = firstTimeToFilter;
                            totalScheduledDowntime.Add(scheduledStopDto.EndTime.Subtract(scheduledStopDto.StartTime));
                        }
                    }
                    else if (scheduledStopDto.EndTime >= lastTimeToFilter && scheduledStopDto.StartTime < lastTimeToFilter)
                    {
                        if (scheduledStopDto.StartTime <= firstTimeToFilter)
                        {
                            var timeToAdd = scheduledStopDto.EndTime <= lastTimeToFilter ? scheduledStopDto.EndTime.Subtract(scheduledStopDto.StartTime) : scheduledStopDto.EndTime.Subtract(firstTimeToFilter);
                            totalScheduledDowntime.Add(timeToAdd);
                        }
                        else
                        {
                            scheduledStopDto.EndTime = lastTimeToFilter;
                            totalScheduledDowntime.Add(scheduledStopDto.EndTime.Subtract(scheduledStopDto.StartTime));
                        }
                    }
                }

            totalScheduledDowntime.Subtract(firstScheduledDowntimeTime);

            if (lastScheduledDowntimeTime > lastTimeToFilter)
                lastScheduledDowntimeTime = lastTimeToFilter;

            var totalCurrentScheduledDowntime = 0;
            var currentDateTime = DateTime.Now;

            var totalCurrentCleanShiftTime = currentDateTime > startDateTime.Date + finalSearchTime ?
                totalSearchTime.TotalMinutes - totalCurrentScheduledDowntime :
                currentDateTime.TimeOfDay.Subtract(initialSearchTime).TotalMinutes - totalCurrentScheduledDowntime;

            #endregion

            #region Filter

            if (isFiltered)
            {
                var machineOperationsToFilter = await machineOperationRepository.GetMachineOperationsToFilter([machineScheduleDto.MachineScheduleId], assetId, startDateTime, endDateTime, shiftType: shiftType, firstShiftDtoTime);

                if (machineOperationsToFilter != null)
                {
                    foreach (var machineOperationToFilter in machineOperationsToFilter)
                    {
                        if (isOptoClass && machineOperationToFilter.TotalProduction != -1)
                        {
                            productionQuantity -= machineOperationToFilter.TotalProduction;
                            dirtyQuantity -= machineOperationToFilter.TotalDirty;
                            leakedQuantity -= machineOperationToFilter.TotalLeaked;
                            crackedOptoQuantity -= machineOperationToFilter.TotalCracked;
                            brokenQuantity -= machineOperationToFilter.TotalBroken;
                        }
                        else
                            productionQuantity = totalProductionMagna -= machineOperationToFilter.EggQuantitiesDto
                                .FirstOrDefault(e => e.Type == EEggQuantityType.Production)?
                                .SumEggQuantityProperties() ?? 0;

                        if (machineOperationToFilter.EggQuantitiesDto.Any())
                            crackedMagnaQuantity -= machineOperationToFilter.EggQuantitiesDto
                                .FirstOrDefault(e => e.Type == EEggQuantityType.Cracked)?
                                .SumEggQuantityProperties() ?? 0;
                    }
                }

                if (productionQuantity < 0 || dirtyQuantity < 0 || leakedQuantity < 0 || crackedOptoQuantity < 0 || crackedMagnaQuantity < 0 || brokenQuantity < 0)
                    throw new InvalidOperationException("ErrorApplyingFilters");
            }

            #endregion

            #region Production schedule

            var productionSchedule = MapToProductionSchedule(machineOperationsDto, scheduledStopsDto, totalSearchTime, initialSearchTime, finalSearchTime, startDateTime.Date);

            #endregion

            #region Runtimes

            var stoppedMachineData = machineOperationsDto.Where(r =>
                r.MachineStatus == EMachineStatus.Emergency || r.MachineStatus == EMachineStatus.StandBy ||
                r.MachineStatus == EMachineStatus.LoadCellError || r.MachineStatus == EMachineStatus.InsufficientOutputs ||
                r.MachineStatus == EMachineStatus.SolenoidError);

            var runtimeResults = MapToRuntimes(stoppedMachineData, totalSearchTime, TimeSpan.FromMinutes(totalOperationTime), out TimeSpan totalUnscheduledDowntime);

            #endregion

            #region Performance and quality

            double oee = 0;

            var machineCapacity = configuration.GetValue<double>("MachineCapacity");

            if (totalProductionMagna != 0 && machineCapacity != 0 && totalCurrentCleanShiftTime != 0)
                oee = (totalProductionMagna - crackedMagnaQuantity - crackedOptoQuantity) / (machineCapacity * 6 * totalCurrentCleanShiftTime) * 100;

            double availability, quality, performance;
            availability = quality = performance = 0;

            if (oee > 0)
            {
                availability = GetAvailabilityPercentage(totalOperationTime, totalCurrentCleanShiftTime);
                quality = GetQualityPercentage(productionQuantity, leakedQuantity + brokenQuantity + crackedOptoQuantity + dirtyQuantity + crackedMagnaQuantity);
                performance = GetPerformancePercentage(oee, availability, quality);
            }

            var performanceAndQuality = new PerformanceAndQuality
            {
                Availability = $"{availability.ToFormatedDecimal()}%",
                Performance = $"{performance.ToFormatedDecimal()}%",
                Quality = $"{quality.ToFormatedDecimal()}%"
            };

            #endregion

            #region Effectiveness

            var machineSpeedList = machineOperationsDto
                .Where(m => m.MachineStatus == EMachineStatus.Operation)
                .Select(m => new MachineSpeed(m.ProgSpeed, m.CurrentSpeed));

            var progSpeedAverage = machineSpeedList.Average(m => m.ProgSpeed) ?? 0;
            double averageProductionValue = 0;

            if (totalOperationTime != 0 && progSpeedAverage != 0)
                averageProductionValue = Math.Round((double)productionQuantity / 360 /
                (totalOperationTime / 60), 1);

            var averageProduction = new AverageProduction
            {
                TotalSize = averageProductionValue > machineCapacity ? averageProductionValue : machineCapacity,
                Value = averageProductionValue
            };

            var effectivenessResult = new Effectiveness
            {
                AverageProduction = averageProduction,
                Fill = fill,
                Oee = oee
            };

            #endregion

            #region Total data

            var boxPerHourTranslation = Translator.Translate("BoxPerHour");
            var currentSpeedAverage = machineSpeedList.Average(m => m.CurrentSpeed) ?? 0;

            var totalData = new TotalData
            {
                RealProduction = $"{Math.Round((double)productionQuantity / 360, 2).ToFormatedDecimal()}  {Translator.Translate("Box")}",
                AverageRealSpeed = $"{currentSpeedAverage.ToFormatedDecimal()} {boxPerHourTranslation}",
                IdealSpeed = $"{progSpeedAverage.ToFormatedDecimal()} {boxPerHourTranslation}",
                ScheduledStops = totalScheduledDowntime.ToStringTime(),
                HoursAvailable = totalSearchTime.Subtract(totalScheduledDowntime).ToStringTime()
            };

            #endregion

            return new ProductionResultsResponse
            {
                EndDate = endDateTime?.ToStringBrFormat(),
                ProductionSchedule = productionSchedule,
                PerformanceAndQuality = performanceAndQuality,
                Effectiveness = effectivenessResult,
                Runtimes = runtimeResults,
                TotalData = totalData,
            };
        }

        public async Task<EggResultsResponse> GetEggResults(DateTime startDateTime, DateTime endDateTime, bool isOptoClass, EShiftType? shiftType, int[]? customerIds)
        {
            var userId = HttpContextHelper.GetUserId();
            var assetId = await userRepository.GetAssetIdById(userId)
                ?? throw new InvalidOperationException("AssetNotFound");

            var machineSchedulesDto = await machineScheduleRepository.GetFilteredMachineSchedules(userId, startDateTime, endDateTime, shiftType)
                ?? throw new InvalidOperationException("NoMachineScheduleFound");

            machineSchedulesDataDtoValidator.Validate(machineSchedulesDto);

            var machineScheduleIds = machineSchedulesDto.Select(m => m.MachineScheduleId);

            var machineOperationsDto = await machineOperationRepository.GetLastestMachineOperationsByFilters(machineScheduleIds, assetId, startDateTime, endDateTime, shiftType) ?? throw new ValidationException("NoMachineOperationFound");

            if (!machineOperationsDto.Any())
                throw new ValidationException("NoMachineOperationFound");

            #region Necessary properties

            var isFiltered = shiftType.HasValue || customerIds != null;

            if (shiftType.HasValue && machineSchedulesDto.Any(m => m.HasShifts()))
            {
                var shiftFirstTime = machineSchedulesDto.First().FirstShiftDto.StartTime;
                var shiftLastTime = machineSchedulesDto.Last().LastShiftDto.EndTime;

                startDateTime = new DateTime(startDateTime.Year, startDateTime.Month, startDateTime.Day,
                                        shiftFirstTime.Hours, shiftFirstTime.Minutes, shiftFirstTime.Seconds);

                endDateTime = new DateTime(startDateTime.Year, startDateTime.Month, startDateTime.Day,
                                    shiftLastTime.Hours, shiftLastTime.Minutes, shiftLastTime.Seconds);
            }

            var eggQuantitiesDto = machineOperationsDto
                .Select(r => r.EggQuantitiesDto).ToList();

            int productionQuantity, eggsWeightQuantity, badQuantity, dirtyQuantity, leakedQuantity, crackedOptoQuantity, brokenQuantity;
            productionQuantity = eggsWeightQuantity = badQuantity = dirtyQuantity = leakedQuantity = crackedOptoQuantity = brokenQuantity = 0;

            foreach (var eggQuantityDto in eggQuantitiesDto)
            {
                productionQuantity += eggQuantityDto?
                        .FirstOrDefault(e => e.Type == EEggQuantityType.Production)?
                        .SumEggQuantityProperties() ?? 0;

                eggsWeightQuantity += eggQuantityDto?
                    .FirstOrDefault(e => e.Type == EEggQuantityType.Weight)?
                    .SumEggQuantityProperties() ?? 0;
            }

            var whiteQuantity = productionQuantity;

            if (isOptoClass)
            {
                productionQuantity = 0;

                for (var i = 0; i < machineOperationsDto.Count; i++)
                {
                    var prodQuantityMagna = eggQuantitiesDto[i]?
                        .FirstOrDefault(e => e.Type == EEggQuantityType.Production)?
                        .SumEggQuantityProperties() ?? 0;

                    var prodQuantityOpto = machineOperationsDto[i]?.TotalProduction ?? 0;

                    productionQuantity += prodQuantityOpto >= prodQuantityMagna ?
                        prodQuantityOpto : prodQuantityMagna;
                }

                whiteQuantity = machineOperationsDto?.Sum(l => l.TotalWhite) ?? 0;
                badQuantity = machineOperationsDto?.Sum(l => l.TotalBad) ?? 0;
                dirtyQuantity = machineOperationsDto?.Sum(l => l.TotalDirty) ?? 0;
                leakedQuantity = machineOperationsDto?.Sum(l => l.TotalLeaked) ?? 0;
                brokenQuantity = machineOperationsDto?.Sum(l => l.TotalBroken) ?? 0;
                crackedOptoQuantity = machineOperationsDto?.Sum(l => l.TotalCracked) ?? 0;
            }

            #endregion

            #region Filter

            if (isFiltered)
            {
                var machineOperationsToFilter = await machineOperationRepository.GetMachineOperationsToFilter(machineScheduleIds, assetId, startDateTime, endDateTime, shiftType, TimeSpan.Zero);

                if (machineOperationsToFilter != null)
                {
                    foreach (var machineOperationToFilter in machineOperationsToFilter)
                    {
                        if (isOptoClass && machineOperationToFilter.TotalProduction != -1)
                        {
                            productionQuantity -= machineOperationToFilter.TotalProduction;
                            dirtyQuantity -= machineOperationToFilter.TotalDirty;
                            leakedQuantity -= machineOperationToFilter.TotalLeaked;
                            crackedOptoQuantity -= machineOperationToFilter.TotalCracked;
                            brokenQuantity -= machineOperationToFilter.TotalBroken;
                            whiteQuantity -= machineOperationToFilter.TotalWhite;
                            badQuantity -= machineOperationToFilter.TotalBad;
                        }
                        else
                            whiteQuantity = productionQuantity -= machineOperationToFilter.EggQuantitiesDto
                                .FirstOrDefault(e => e.Type == EEggQuantityType.Production)?
                                .SumEggQuantityProperties() ?? 0;

                        if (eggQuantitiesDto != null && machineOperationToFilter.EggQuantitiesDto.Any())
                        {
                            var lastEggsQuantitiesDtoList = machineOperationToFilter.EggQuantitiesDto.ToList();

                            foreach (var eggQuantity in eggQuantitiesDto)
                            {
                                if (eggQuantitiesDto != null && lastEggsQuantitiesDtoList != null)
                                    for (var i = 0; i < 3; i++)
                                    {
                                        var indexByType = lastEggsQuantitiesDtoList.FindIndex(l => l.Type == eggQuantity[i].Type);

                                        eggQuantity[i].P1 -= lastEggsQuantitiesDtoList[indexByType].P1;
                                        eggQuantity[i].P2 -= lastEggsQuantitiesDtoList[indexByType].P2;
                                        eggQuantity[i].P3 -= lastEggsQuantitiesDtoList[indexByType].P3;
                                        eggQuantity[i].P4 -= lastEggsQuantitiesDtoList[indexByType].P4;
                                        eggQuantity[i].P5 -= lastEggsQuantitiesDtoList[indexByType].P5;
                                        eggQuantity[i].P6 -= lastEggsQuantitiesDtoList[indexByType].P6;
                                        eggQuantity[i].P7 -= lastEggsQuantitiesDtoList[indexByType].P7;

                                        if (eggQuantity[i].HasValueBelowZero())
                                            throw new InvalidOperationException("ErrorApplyingFilters");
                                    }
                            }

                            if (productionQuantity < 0 || dirtyQuantity < 0 || leakedQuantity < 0 || crackedOptoQuantity < 0 || brokenQuantity < 0 || whiteQuantity < 0 || badQuantity < 0)
                                throw new InvalidOperationException("ErrorApplyingFilters");
                        }
                    }
                }
            }

            #endregion

            #region Total eggs data

            var eggsWeightString = $"{eggsWeightQuantity.ToFormatedInt()} g";

            if (eggsWeightQuantity > 1000)
                eggsWeightString = $"{((double)eggsWeightQuantity / 1000).ToFormatedDecimal()} kg";

            var boxesQuantity = Math.Round((double)productionQuantity / 360, 4).ToFormatedDecimal();

            var totalEggsData = new TotalEggsData
            {
                EggsWeight = eggsWeightString,
                EggsQuantity = $"{productionQuantity} {Translator.Translate("Eggs")}",
                BoxesQuantity = $"{boxesQuantity} {Translator.Translate("Box")}",
            };

            #endregion

            #region General production data

            var generalProductionData = await MapToGeneralProductionData(userId, eggQuantitiesDto, productionQuantity, whiteQuantity, crackedOptoQuantity, isOptoClass);

            #endregion

            #region Vision system statistics

            int[] visionSystemQuantitiesList = [whiteQuantity, productionQuantity - whiteQuantity, dirtyQuantity, crackedOptoQuantity, leakedQuantity, brokenQuantity];

            var visionSystemStatistics = MapToVisionSystemStatistics(visionSystemQuantitiesList, productionQuantity, isOptoClass);

            #endregion

            #region Boxes quantity

            var boxesQuantityChart = MapToBoxesQuantity(generalProductionData);

            #endregion

            return new EggResultsResponse
            {
                TotalEggsData = totalEggsData,
                GeneralProductionData = generalProductionData,
                VisionSystemStatistics = visionSystemStatistics,
                BoxesQuantity = boxesQuantityChart
            };
        }

        public async Task<IEnumerable<GeneralProductionData>> GetDefaultEggCategories(bool isOptoClass)
        {
            var response = new List<GeneralProductionData>(9);

            var eggsTranslation = Translator.Translate("Eggs");
            var boxTranslation = Translator.Translate("Box");

            if (isOptoClass)
            {
                response.Add(new GeneralProductionData
                {
                    EggName = Translator.Translate(EEggType.White.GetName()),
                    EggWeight = "-",
                    EggQuantity = $"0 {eggsTranslation}",
                    BoxQuantity = $"00.0000 {boxTranslation}"
                });

                response.Add(new GeneralProductionData
                {
                    EggName = Translator.Translate(EEggType.Red.GetName()),
                    EggWeight = "-",
                    EggQuantity = $"0 {eggsTranslation}",
                    BoxQuantity = $"00.0000 {boxTranslation}"
                });
            }

            response.Add(new GeneralProductionData
            {
                EggName = Translator.Translate(EEggType.Cracked.GetName()),
                EggWeight = "-",
                EggQuantity = isOptoClass ? $"0 {eggsTranslation}" : "-",
                BoxQuantity = isOptoClass ? $"00.0000 {boxTranslation}" : "-"
            });

            var eggCategoriesDto = await eggCategoryRepository.GetEggCategoriesDtoByUserId(HttpContextHelper.GetUserId());

            foreach (var category in eggCategoriesDto)
            {
                response.Add(new GeneralProductionData
                {
                    EggName = category.Name.ToSafeValue(),
                    EggWeight = "00.00 g",
                    EggQuantity = $"0 {eggsTranslation}",
                    BoxQuantity = $"00.0000 {boxTranslation}"
                });
            }

            return response;
        }

        private static MachineOperationDto AdjustLastOperationValues(MachineOperationDto machineOperation)
        {
            var properties = typeof(MachineOperationDto).GetProperties();

            foreach (var property in properties)
            {
                if (property.PropertyType == typeof(int))
                {
                    var value = (int)property.GetValue(machineOperation)!;

                    if (value == -1)
                        property.SetValue(machineOperation, 0);
                }
                else if (property.PropertyType == typeof(double) || property.PropertyType == typeof(double?))
                {
                    double? value = (double?)property.GetValue(machineOperation);

                    if (value.HasValue && value.Value == -1)
                        property.SetValue(machineOperation, (double?)0);
                }
            }

            return machineOperation;
        }

        private static double GetSumOperationTime(IList<MachineOperationDto> machineOperationsDto)
        {
            double operationTime = 0;
            TimeSpan? startTime = null;

            for (var i = 0; i < machineOperationsDto.Count; i++)
            {
                if (machineOperationsDto[i].MachineStatus == EMachineStatus.Operation && !startTime.HasValue)
                    startTime = machineOperationsDto[i].StartTime;
                else if (machineOperationsDto[i].MachineStatus != EMachineStatus.Operation && startTime.HasValue)
                {
                    operationTime += machineOperationsDto[i].EndTime.Subtract(startTime.Value).TotalMinutes;
                    startTime = null;
                }
            }

            if (startTime.HasValue)
                operationTime += machineOperationsDto.Last().EndTime.Subtract(startTime.Value).TotalMinutes;

            return operationTime;
        }

        private IEnumerable<BoxesQuantity> MapToBoxesQuantity(IEnumerable<GeneralProductionData> generalProductionData) =>
            mapper.ProjectTo<BoxesQuantity>(generalProductionData.AsQueryable()).AsEnumerable();

        private static List<VisionSystemStatistics> MapToVisionSystemStatistics(int[] visionSystemQuantitiesList, int productionQuantity, bool isOptoClass)
        {
            if (!isOptoClass)
                return new List<VisionSystemStatistics>(6);

            var response = new List<VisionSystemStatistics>();

            for (var i = 0; i < visionSystemQuantitiesList.Length; i++)
            {
                response.Add(
                    new VisionSystemStatistics
                    {
                        EggName = $"{Translator.Translate(((EVisonSystemType)i).GetName())}:",
                        EggPercentage = GetDoublePercentage(visionSystemQuantitiesList[i], productionQuantity)
                    });
            };

            return response;
        }

        private async Task<List<GeneralProductionData>> MapToGeneralProductionData(int userId, List<IList<EggQuantityDto>>? eggQuantitiesList, int productionQuantity, int whiteQuantity, int crackedOptoQuantity, bool isOptoClass)
        {
            var response = new List<GeneralProductionData>(9);

            var eggsTranslation = Translator.Translate("Eggs");
            var boxTranslation = Translator.Translate("Box");

            if (isOptoClass)
            {
                response.Add(new GeneralProductionData
                {
                    EggName = Translator.Translate(EEggType.White.GetName()),
                    EggWeight = "-",
                    EggQuantity = $"{whiteQuantity} {eggsTranslation}",
                    BoxQuantity = $"{Math.Round((double)whiteQuantity / 360, 4).ToFormatedDecimal("00.0000")} {boxTranslation}"
                });

                response.Add(new GeneralProductionData
                {
                    EggName = Translator.Translate(EEggType.Red.GetName()),
                    EggWeight = "-",
                    EggQuantity = $"{productionQuantity - whiteQuantity} {eggsTranslation}",
                    BoxQuantity = $"{Math.Round((double)(productionQuantity - whiteQuantity) / 360, 4).ToFormatedDecimal("00.0000")} {boxTranslation}"
                });
            }

            var currentCategories = await eggCategoryRepository.GetEggCategoriesDtoByUserId(userId);

            if (eggQuantitiesList != null && eggQuantitiesList.Count != 0)
            {
                var crackedQuantity = crackedOptoQuantity;

                if (isOptoClass)
                    crackedQuantity += eggQuantitiesList.Sum(e => e.FirstOrDefault(e => e.Type == EEggQuantityType.Cracked)?.SumEggQuantityProperties()) ?? 0;

                response.Add(new GeneralProductionData
                {
                    EggName = Translator.Translate(EEggType.Cracked.GetName()),
                    EggWeight = "-",
                    EggQuantity = $"{crackedQuantity} {eggsTranslation}",
                    BoxQuantity = $"{Math.Round((double)crackedQuantity / 360, 4).ToFormatedDecimal("00.0000")} {boxTranslation}"
                });

                foreach (var category in currentCategories)
                {
                    int eggWheight, eggQuantity;
                    eggWheight = eggQuantity = 0;

                    foreach (var eggQuantities in eggQuantitiesList)
                    {
                        if (eggQuantities != null)
                        {
                            eggWheight += eggQuantities
                                .Where(e => e.Type == EEggQuantityType.Weight)
                                .Sum(e => int.Parse(GetEggQuantityValue(e, category.Category.GetName())));

                            eggQuantity += eggQuantities
                                .Where(e => e.Type == EEggQuantityType.Production)
                                .Sum(e => int.Parse(GetEggQuantityValue(e, category.Category.GetName())));
                        }
                    }

                    var eggWeightString = eggWheight < 1000 ? $"{eggWheight.ToFormatedInt()} g" :
                           $"{((double)eggWheight / 1000).ToFormatedDecimal()} kg";

                    var eggQuantityString = $"{eggQuantity} {eggsTranslation}";

                    double boxQuantity = Math.Round((double)eggQuantity / 360, 4);
                    var boxQuantityString = $"{boxQuantity.ToFormatedDecimal("00.0000")} {boxTranslation}";

                    response.Add(new GeneralProductionData
                    {
                        EggName = category.Name.ToSafeValue(),
                        EggWeight = eggWeightString,
                        EggQuantity = eggQuantityString,
                        BoxQuantity = boxQuantityString
                    });
                }
            }
            else
                foreach (var category in currentCategories)
                {
                    response.Add(new GeneralProductionData
                    {
                        EggName = category.Name.ToSafeValue(),
                        EggWeight = "00.00 kg",
                        EggQuantity = $"00 {eggsTranslation}",
                        BoxQuantity = $"00.00{boxTranslation}"
                    });
                }

            return response;
        }

        private static string GetEggQuantityValue(EggQuantityDto eggQuantity, string propertyName)
        {
            var propertyInfo = typeof(EggQuantityDto).GetProperty(propertyName);

            if (propertyInfo != null)
                return propertyInfo.GetValue(eggQuantity)?.ToString() ?? "0";

            return "0";
        }

        private static double GetAvailabilityPercentage(double operationTime, double totalCleanShiftTime)
        {
            if (totalCleanShiftTime == 0)
                return 0;

            return operationTime / totalCleanShiftTime * 100;
        }

        private static double GetQualityPercentage(int productionQuantity, int totalDefects)
        {
            if (productionQuantity == 0)
                return 0;

            return (double)(productionQuantity - totalDefects) / productionQuantity * 100;
        }

        private static double GetPerformancePercentage(double oee, double availability, double quality)
        {
            if (availability == 0 || quality == 0)
                return 0;

            return oee / (availability * quality) * 10000;
        }

        private List<ProductionSchedule> MapToProductionSchedule(IList<MachineOperationDto> machineOperationsDto, IEnumerable<ScheduledStopDto> scheduledStops, TimeSpan totalSearchTime, TimeSpan initialSearchTime, TimeSpan finalSearchTime, DateTime startDateTime)
        {
            machineOperationsDto = GroupByStatus(machineOperationsDto);

            if (machineOperationsDto.Last().EndTime > finalSearchTime)
                machineOperationsDto.Last().EndTime = finalSearchTime;

            var productionSchedule = mapper.ProjectTo<ProductionSchedule>(machineOperationsDto.AsQueryable()).ToList();

            var totalMachineRealTime = TimeSpan.Zero;

            for (var i = 0; i < machineOperationsDto.Count; i++)
            {
                if (scheduledStops.Any() && machineOperationsDto[i].MachineStatus == EMachineStatus.StandBy)
                    if (IsInScheduledStop(machineOperationsDto[i], scheduledStops))
                        machineOperationsDto[i].MachineStatus = EMachineStatus.ScheduledStop;

                var totalStatusHour = machineOperationsDto[i].EndTime.Subtract(machineOperationsDto[i].StartTime);
                totalMachineRealTime += totalStatusHour;
                productionSchedule[i].Width = GetStringPercentage(totalStatusHour, totalSearchTime);

                if (machineOperationsDto[i].MachineStatus == EMachineStatus.LoadCellError ||
                    machineOperationsDto[i].MachineStatus == EMachineStatus.SolenoidError ||
                    machineOperationsDto[i].MachineStatus == EMachineStatus.InsufficientOutputs)
                    productionSchedule[i].ProductionScheduleStatus = EProductionScheduleStatus.UnscheduledStop;

                else if (machineOperationsDto[i].MachineStatus != EMachineStatus.StandBy)
                    productionSchedule[i].ProductionScheduleStatus = Enum.Parse<EProductionScheduleStatus>(machineOperationsDto[i].MachineStatus.ToString());
                else
                    productionSchedule[i].ProductionScheduleStatus = EProductionScheduleStatus.StoppedMachine;

                productionSchedule[i].Color = productionSchedule[i].ProductionScheduleStatus.GetDescription();
                productionSchedule[i].ProductionScheduleName = Translator.Translate(productionSchedule[i].ProductionScheduleStatus.GetName());
            }

            var twoSec = new TimeSpan(0, 0, 2);

            if (machineOperationsDto.First().MachineStatus == EMachineStatus.Off)
            {
                var machineStoppedTotalTime = machineOperationsDto.First().EndTime.Subtract(initialSearchTime);

                productionSchedule[0].StartTime = initialSearchTime.ToStringTime("hh\\:mm\\:ss");
                productionSchedule[0].Width = GetStringPercentage(machineStoppedTotalTime, totalSearchTime);
            }
            else
            {
                var unrecordedInitialTime = machineOperationsDto.First().StartTime.Subtract(initialSearchTime);

                if (unrecordedInitialTime > twoSec)
                    productionSchedule.Insert(0, new ProductionSchedule
                    {
                        MachineStatus = EMachineStatus.Off,
                        ProductionScheduleStatus = EProductionScheduleStatus.Off,
                        MachineStatusName = Translator.Translate(EMachineStatus.Off.GetName()),
                        ProductionScheduleName = Translator.Translate(EProductionScheduleStatus.Off.GetName()),
                        Color = EProductionScheduleStatus.Off.GetDescription(),
                        StartTime = initialSearchTime.ToStringTime("hh\\:mm\\:ss"),
                        EndTime = productionSchedule[0].StartTime,
                        Width = GetStringPercentage(unrecordedInitialTime, totalSearchTime)
                    });
            }

            var currentDateTime = DateTime.Now;
            var lastMachineOperationDateTime = startDateTime + machineOperationsDto.Last().EndTime;
            var isInRealTime = currentDateTime < startDateTime + finalSearchTime;

            TimeSpan machineOffTime, shiftRemainingTime;
            shiftRemainingTime = TimeSpan.Zero;

            if (isInRealTime)
            {
                machineOffTime = currentDateTime.Subtract(lastMachineOperationDateTime);
                shiftRemainingTime = finalSearchTime.Subtract(currentDateTime.TimeOfDay);
            }
            else
                machineOffTime = finalSearchTime.Subtract(machineOperationsDto.Last().EndTime);

            if (machineOffTime > shiftRemainingTime)
                productionSchedule.Add(new ProductionSchedule
                {
                    MachineStatus = EMachineStatus.Off,
                    ProductionScheduleStatus = EProductionScheduleStatus.Off,
                    MachineStatusName = Translator.Translate(EMachineStatus.Off.GetName()),
                    ProductionScheduleName = Translator.Translate(EProductionScheduleStatus.Off.GetName()),
                    Color = EProductionScheduleStatus.Off.GetDescription(),
                    StartTime = productionSchedule.Last().EndTime,
                    EndTime = finalSearchTime.ToStringTime("hh\\:mm\\:ss"),
                    Width = GetStringPercentage(finalSearchTime.Subtract(machineOperationsDto.Last().EndTime), totalSearchTime)
                });
            else
            {
                if (machineOffTime > new TimeSpan(0, 0, 20))
                {
                    productionSchedule.Add(new ProductionSchedule //aqui
                    {
                        MachineStatus = EMachineStatus.Off,
                        ProductionScheduleStatus = EProductionScheduleStatus.Off,
                        MachineStatusName = Translator.Translate(EMachineStatus.Off.GetName()),
                        ProductionScheduleName = Translator.Translate(EProductionScheduleStatus.Off.GetName()),
                        Color = EProductionScheduleStatus.Off.GetDescription(),
                        StartTime = productionSchedule.Last().EndTime,
                        EndTime = currentDateTime.TimeOfDay.ToStringTime("hh\\:mm\\:ss"),
                        Width = GetStringPercentage(machineOffTime, totalSearchTime)
                    });

                    productionSchedule.Add(new ProductionSchedule
                    {
                        MachineStatus = EMachineStatus.RemainingTime,
                        ProductionScheduleStatus = EProductionScheduleStatus.RemainingTime,
                        MachineStatusName = Translator.Translate(EMachineStatus.RemainingTime.GetName()),
                        ProductionScheduleName = Translator.Translate(EProductionScheduleStatus.RemainingTime.GetName()),
                        Color = EProductionScheduleStatus.RemainingTime.GetDescription(),
                        StartTime = productionSchedule.LastOrDefault() != null ? productionSchedule.Last().EndTime : currentDateTime.TimeOfDay.ToStringTime("hh\\:mm\\:ss"),
                        EndTime = finalSearchTime.ToStringTime("hh\\:mm\\:ss"),
                        Width = GetStringPercentage(shiftRemainingTime, totalSearchTime)
                    });
                }
                else if (shiftRemainingTime > twoSec)
                    productionSchedule.Add(new ProductionSchedule
                    {
                        MachineStatus = EMachineStatus.RemainingTime,
                        ProductionScheduleStatus = EProductionScheduleStatus.RemainingTime,
                        MachineStatusName = Translator.Translate(EMachineStatus.RemainingTime.GetName()),
                        ProductionScheduleName = Translator.Translate(EProductionScheduleStatus.RemainingTime.GetName()),
                        Color = EProductionScheduleStatus.RemainingTime.GetDescription(),
                        StartTime = productionSchedule.Last().EndTime,
                        EndTime = finalSearchTime.ToStringTime("hh\\:mm\\:ss"),
                        Width = GetStringPercentage(shiftRemainingTime, totalSearchTime)
                    });
            }

            return productionSchedule;
        }

        private static bool IsInScheduledStop(MachineOperationDto machineOperation, IEnumerable<ScheduledStopDto> scheduledStops)
        {
            foreach (var scheduledStop in scheduledStops)
            {
                if ((machineOperation.StartTime <= scheduledStop.StartTime && machineOperation.EndTime > scheduledStop.EndTime) ||
                    (machineOperation.StartTime >= scheduledStop.StartTime && machineOperation.EndTime <= scheduledStop.EndTime) ||
                    (machineOperation.StartTime < scheduledStop.StartTime && machineOperation.EndTime >= scheduledStop.EndTime))
                    return true;
            }

            return false;
        }

        private static List<MachineOperationDto> GroupByStatus(IList<MachineOperationDto> machineOperationsDto)
        {
            var response = new List<MachineOperationDto>();
            var currentReport = machineOperationsDto[0];

            foreach (var machineOperationDto in machineOperationsDto)
            {
                if (machineOperationDto.MachineStatus != currentReport.MachineStatus)
                {
                    response.Add(currentReport);
                    currentReport = machineOperationDto;
                }
                else
                    currentReport.EndTime = machineOperationDto.EndTime;
            }

            response.Add(currentReport);

            return response;
        }

        private static List<Runtime> MapToRuntimes(IEnumerable<MachineOperationDto> machineOperationDto, TimeSpan totalShiftTime, TimeSpan totalMachineProducingTime, out TimeSpan totalUnscheduledDowntime)
        {
            var response = new List<Runtime>(7);
            ERuntimeStatus[] runtimesList =
            {
                ERuntimeStatus.Emergency, ERuntimeStatus.StoppedMachine, ERuntimeStatus.Scale, ERuntimeStatus.InsufficientOutputs,
                ERuntimeStatus.Solenoid, ERuntimeStatus.TotalUnscheduledDowntime, ERuntimeStatus.TotalMachineProducing
            };

            foreach (var runtimeStatus in runtimesList)
            {
                response.Add(
                    new Runtime
                    {
                        RuntimeStatus = runtimeStatus,
                        RuntimeName = Translator.Translate(runtimeStatus.GetName()),
                        Hour = "00:00",
                        Percentage = "00.00%"
                    });
            }

            totalUnscheduledDowntime = TimeSpan.FromSeconds
                (machineOperationDto.Sum(item => item.EndTime.TotalMinutes - item.StartTime.TotalMinutes));

            response[5] = new Runtime
            {
                RuntimeStatus = ERuntimeStatus.TotalUnscheduledDowntime,
                RuntimeName = Translator.Translate(ERuntimeStatus.TotalUnscheduledDowntime.GetName()),
                Hour = totalUnscheduledDowntime.ToStringTime(),
                Percentage = GetStringPercentage(totalUnscheduledDowntime, totalShiftTime)
            };

            response[6] = new Runtime
            {
                RuntimeStatus = ERuntimeStatus.TotalMachineProducing,
                RuntimeName = Translator.Translate(ERuntimeStatus.TotalMachineProducing.GetName()),
                Hour = totalMachineProducingTime.ToStringTime(),
                Percentage = GetStringPercentage(totalMachineProducingTime, totalShiftTime)
            };

            for (int i = 0; i < Enum.GetNames(typeof(EMachineStatus)).Length; i++)
            {
                var fieldHour = GetTotalTime(machineOperationDto, (EMachineStatus)i);
                var fieldPercentageString = GetStringPercentage(fieldHour, totalShiftTime);

                if (fieldHour != TimeSpan.Zero)
                {
                    var indexToAdd = response.FindIndex(r => r.RuntimeStatus == (ERuntimeStatus)i);

                    response[indexToAdd] =
                        new Runtime
                        {
                            RuntimeStatus = (ERuntimeStatus)i,
                            RuntimeName = Translator.Translate(((ERuntimeStatus)i).GetName()),
                            Hour = fieldHour.ToStringTime(),
                            Percentage = fieldPercentageString
                        };
                }
            }

            return response;
        }

        private static string GetStringPercentage(TimeSpan fieldHour, TimeSpan totalShiftTime)
        {
            if (totalShiftTime.TotalMinutes != 0)
                return $"{(fieldHour.TotalMinutes / totalShiftTime.TotalMinutes * 100).ToFormatedDecimal()}%";

            return "00.00%";
        }

        private static TimeSpan GetTotalTime(IEnumerable<MachineOperationDto> machineOperations, EMachineStatus machineStatus)
        {
            var totalTime = TimeSpan.Zero;

            foreach (var operation in machineOperations)
            {
                if (operation.MachineStatus == machineStatus)
                {
                    var operationDuration = operation.EndTime - operation.StartTime;
                    totalTime += operationDuration;
                }
            }

            return totalTime;
        }

        private static double GetDoublePercentage(int eggQuantity, int totalEggProduction)
        {
            if (totalEggProduction != 0)
                return Math.Round((double)eggQuantity / totalEggProduction * 100, 2);

            return 0;
        }
    }
}
