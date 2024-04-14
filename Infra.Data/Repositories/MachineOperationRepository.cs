using Domain.Dto_s.Machine;
using Domain.Interfaces.Repositories;
using Domain.Models;
using Domain.Models.Enums.Scheduling;
using Domain.Objects.Dto_s.Egg;
using Domain.Utils.Helpers;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Infra.Data.Repositories
{
    public class MachineOperationRepository(IMongoClient mongoClient) : BaseNoSqlRepository<MachineOperation>(mongoClient), IMachineOperationRepository
    {
        public async Task<TimeSpan?> GetLastEndTimeByMachineScheduleId(int machineScheduleId)
        {
            var result = await _collection
                   .Find(m => m.MachineScheduleId == machineScheduleId)
                   .Limit(1)
                   .SortByDescending(m => m.InsertedAt)
                   .Project(m => m.EndTime)
                   .ToListAsync();

            if (result.Count == 0)
                return null;

            return result.FirstOrDefault();
        }

        public async Task<IList<MachineOperationDto>?> GetFirstAndLastMachineOperationsByFilters(int machineScheduleId, int assetId, DateTime startDateTime, DateTime? endDateTime, EShiftType? shiftType)
        {
            var filter = _filterBuilder.Eq(m => m.MachineScheduleId, machineScheduleId) &
                _filterBuilder.Eq(m => m.AssetId, assetId) &
                _filterBuilder.Gte(m => m.InsertedAt, startDateTime.ToMinDateTime()) &
                _filterBuilder.Lte(m => m.InsertedAt, startDateTime.ToMaxDateTime());

            if (endDateTime.HasValue)
                filter &= _filterBuilder.Gte(m => m.StartTime, startDateTime.TimeOfDay) &
                    _filterBuilder.Lte(m => m.StartTime, endDateTime.Value.TimeOfDay);

            if (shiftType.HasValue)
                filter &= _filterBuilder.Eq(m => m.ShiftType, shiftType.Value);

            #region Projection

            var projection = Builders<MachineOperation>.Projection.Expression(m => new MachineOperationDto
            {
                Id = m.Id,
                MachineStatus = m.MachineStatus,
                StartTime = m.StartTime,
                EndTime = m.EndTime,
                CurrentSpeed = m.CurrentSpeed,
                Fill = m.Fill,
                ProgSpeed = m.ProgSpeed,
                TotalProduction = m.TotalProduction,
                TotalCracked = m.TotalCracked,
                TotalDirty = m.TotalDirty,
                TotalLeaked = m.TotalLeaked,
                TotalBroken = m.TotalBroken,
                EggQuantitiesDto = m.EggQuantities.Select(e => new EggQuantityDto
                {
                    Type = e.Type,
                    P1 = e.P1,
                    P2 = e.P2,
                    P3 = e.P3,
                    P4 = e.P4,
                    P5 = e.P5,
                    P6 = e.P6,
                    P7 = e.P7,
                }).ToList()
            });

            #endregion

            return await _collection.Find(filter)
                .Project(projection)
                .ToListAsync();
        }

        public async Task<IList<MachineOperationDto>?> GetLastestMachineOperationsByFilters(IEnumerable<int> machineScheduleIds, int assetId, DateTime startDateTime, DateTime endDateTime, EShiftType? shiftType)
        {
            var filter = _filterBuilder.In(m => m.MachineScheduleId, machineScheduleIds) &
                _filterBuilder.Eq(m => m.AssetId, assetId) &
                _filterBuilder.Gte(m => m.InsertedAt, startDateTime.ToMinDateTime()) &
                _filterBuilder.Lte(m => m.InsertedAt, endDateTime.ToMaxDateTime()) &
                _filterBuilder.Gte(m => m.StartTime, startDateTime.TimeOfDay) &
                _filterBuilder.Lte(m => m.EndTime, endDateTime.TimeOfDay);

            if (shiftType.HasValue)
                filter &= _filterBuilder.Eq(m => m.ShiftType, shiftType.Value);

            #region Projection

            var projection = Builders<MachineOperation>.Projection.Expression(m => new MachineOperationDto
            {
                Id = m.Id,
                TotalProduction = m.TotalProduction,
                TotalBad = m.TotalBad,
                TotalWhite = m.TotalWhite,
                TotalCracked = m.TotalCracked,
                TotalDirty = m.TotalDirty,
                TotalLeaked = m.TotalLeaked,
                TotalBroken = m.TotalBroken,
                EggQuantitiesDto = m.EggQuantities.Select(e => new EggQuantityDto
                {
                    Type = e.Type,
                    P1 = e.P1,
                    P2 = e.P2,
                    P3 = e.P3,
                    P4 = e.P4,
                    P5 = e.P5,
                    P6 = e.P6,
                    P7 = e.P7,
                }).ToList()
            });

            #endregion

            return await _collection
                .Aggregate()
                .Match(filter)
                .SortByDescending(m => m.InsertedAt)
                .Group(m => m.MachineScheduleId,
                    g => new
                    {
                        LastDocument = g.First()
                    })
                .ReplaceRoot(m => m.LastDocument)
                .SortByDescending(m => m.InsertedAt)
                .Project(projection)
                .ToListAsync();
        }

        public async Task<IEnumerable<MachineOperationDataDto>?> GetMachineOperationsToFilter(IEnumerable<int> machineScheduleIds, int assetId, DateTime startDateTime, DateTime? endDateTime, EShiftType? shiftType, TimeSpan firstShiftDtoTime)
        {
            var filter = _filterBuilder.In(m => m.MachineScheduleId, machineScheduleIds) &
                _filterBuilder.Eq(m => m.AssetId, assetId);

            if (shiftType.HasValue)
            {
                if (endDateTime.HasValue)
                {
                    if (startDateTime.TimeOfDay < firstShiftDtoTime)
                    {
                        if (endDateTime.Value.TimeOfDay > firstShiftDtoTime)
                            filter &= _filterBuilder.Lt(m => m.StartTime, firstShiftDtoTime);
                        else
                            filter &= _filterBuilder.Lt(m => m.StartTime, startDateTime.TimeOfDay);
                    }
                    else
                        filter &= _filterBuilder.Lt(m => m.StartTime, startDateTime.TimeOfDay);
                }
                else
                    filter &= _filterBuilder.Ne(m => m.ShiftType, shiftType.Value);
            }
            else if (endDateTime.HasValue)
                filter &= _filterBuilder.Lte(m => m.StartTime, startDateTime.TimeOfDay);

            #region Projection

            var projection = Builders<MachineOperation>.Projection.Expression(m => new MachineOperationDataDto
            {
                Id = m.Id,
                EndTime = m.EndTime,
                TotalProduction = m.TotalProduction,
                TotalBad = m.TotalBad,
                TotalWhite = m.TotalWhite,
                TotalCracked = m.TotalCracked,
                TotalDirty = m.TotalDirty,
                TotalLeaked = m.TotalLeaked,
                TotalBroken = m.TotalBroken,
                EggQuantitiesDto = m.EggQuantities.Select(e => new EggQuantityDto
                {
                    Type = e.Type,
                    P1 = e.P1,
                    P2 = e.P2,
                    P3 = e.P3,
                    P4 = e.P4,
                    P5 = e.P5,
                    P6 = e.P6,
                    P7 = e.P7,
                })
            });

            #endregion

            return await _collection.Find(filter)
                .Project(projection)
                .SortByDescending(m => m.InsertedAt)
                .Limit(1)
                .ToListAsync();
        }
    }
}
