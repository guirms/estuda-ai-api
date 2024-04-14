using Test.Fixtures;

namespace Test.Tests.Validators
{
    public class MachineScheduleDataDtoValidatorTest : MachineDataFixture
    {
        //private const string TraitName = "MachineScheduleDataDto";
        //private const string HasOperationsAndShifts = "HasOperationsAndShifts";
        //private const string HasAnyOperationOrShift = "HasAnyOperationOrShift";

        //[Fact(DisplayName = "Empty machine operations")]
        //[Trait(TraitName, HasOperationsAndShifts)]
        //internal void EmptyMachineOperation_ThrowsNoMachineOperationFound()
        //{
        //    var methodCall = Assert.Throws<ValidationException>(() =>
        //        _MachineScheduleDataDtoValidator.TestValidate(MachineScheduleDataDtoFaker.GenerateEmptyMachineScheduleDataDto()));

        //    Assert.Equal("NoMachineOperationFound", methodCall.Message);
        //}

        //[Fact(DisplayName = "Empty shifts")]
        //[Trait(TraitName, HasOperationsAndShifts)]
        //internal void EmptyShifts_ThrowsNoShiftFound()
        //{
        //    var machineScheduleDataDto = MachineScheduleDataDtoFaker.GenerateEmptyMachineScheduleDataDto();
        //    machineScheduleDataDto.MachineOperationsDto.Add(Any<MachineOperationDto>());

        //    var methodCall = Assert.Throws<ValidationException>(() =>
        //        _MachineScheduleDataDtoValidator.TestValidate(machineScheduleDataDto));

        //    Assert.Equal("NoShiftFound", methodCall.Message);
        //}

        //[Fact(DisplayName = "Shift with times shorter than current time")]
        //[Trait(TraitName, HasOperationsAndShifts)]
        //internal void ShiftWithTimesShorterThanCurrentTime_ThrowsNoShiftFound()
        //{
        //    var machineScheduleDataDto = MachineScheduleDataDtoFaker.GenerateEmptyMachineScheduleDataDto();
        //    machineScheduleDataDto.MachineOperationsDto.Add(Any<MachineOperationDto>());
        //    machineScheduleDataDto.ShiftsDto.Add(MachineScheduleDataDtoFaker.GenerateShiftWithCurrentTimeShorterThanShiftTime());

        //    var methodCall = Assert.Throws<ValidationException>(() =>
        //        _MachineScheduleDataDtoValidator.TestValidate(machineScheduleDataDto));

        //    Assert.Equal("NoShiftFound", methodCall.Message);
        //}

        //[Fact(DisplayName = "Shift with times greater than current time")]
        //[Trait(TraitName, HasOperationsAndShifts)]
        //internal void ShiftWithTimesGreaterThanCurrentTime_ThrowsNoShiftFound()
        //{
        //    var machineScheduleDataDto = MachineScheduleDataDtoFaker.GenerateEmptyMachineScheduleDataDto();
        //    machineScheduleDataDto.MachineOperationsDto.Add(Any<MachineOperationDto>());
        //    machineScheduleDataDto.ShiftsDto.Add(MachineScheduleDataDtoFaker.GenerateShiftWithCurrentTimeGreaterThanShiftTime());

        //    var methodCall = Assert.Throws<ValidationException>(() =>
        //        _MachineScheduleDataDtoValidator.TestValidate(machineScheduleDataDto));

        //    Assert.Equal("NoShiftFound", methodCall.Message);
        //}

        //[Fact(DisplayName = "Empty machine operations")]
        //[Trait(TraitName, HasAnyOperationOrShift)]
        //internal void EmptyMachineOperations_ThrowsNoMachineOperationFound()
        //{
        //    var methodCall = Assert.Throws<ValidationException>(() =>
        //        _machineSchedulesDtoValidator.TestValidate(MachineScheduleDataDtoFaker.GenerateEmptyMachineSchedulesDataDto()));

        //    Assert.Equal("NoMachineOperationFound", methodCall.Message);
        //}

        //[Fact(DisplayName = "Empty list of shifts")]
        //[Trait(TraitName, HasAnyOperationOrShift)]
        //internal void EmptyShifts_ThrowsNoMachineOperationFound()
        //{
        //    var machineSchedulesDataDto = MachineScheduleDataDtoFaker.GenerateEmptyMachineSchedulesDataDto();
        //    machineSchedulesDataDto.First().MachineOperationsDto.Add(Any<MachineOperationDto>());

        //    var methodCall = Assert.Throws<ValidationException>(() =>
        //        _machineSchedulesDtoValidator.TestValidate(machineSchedulesDataDto));

        //    Assert.Equal("NoShiftFound", methodCall.Message);
        //}
    }
}
