using Test.Fixtures;

namespace Test.Tests.AppServices
{
    public class MachineDataAppServiceTest : MachineDataFixture
    {
        private const string TraitName = "MachineDataAppService";
        private const string GetProductionResults = "GetProductionResults";

        //[Fact(DisplayName = "Start datetime greater than end datetime")]
        //[Trait(TraitName, GetProductionResults)]
        //internal async void StartDatetimeGreaterThanEndDatetime_ThrowsStartDateGreaterThanEndDate()
        //{
        //    var currentDateTime = DateTime.Now;

        //    var startDate = currentDateTime.AddHours(1);
        //    var isOptoClass = Any<bool>();
        //    var isFeeback = Any<bool>();
        //    var endDate = currentDateTime;

        //    var methodCall = await Assert.ThrowsAsync<ValidationException>(() =>
        //        _machineDataAppService.GetProductionResults(startDate, isOptoClass, isFeeback, endDate, null));

        //    Assert.Equal("StartDateGreaterThanEndDate", methodCall.Message);
        //}

        //[Fact(DisplayName = "Different days")]
        //[Trait(TraitName, GetProductionResults)]
        //internal async void DifferentDays_ThrowsDaysCannotBeDifferent()
        //{
        //    var currentDateTime = DateTime.Now;

        //    var startDate = currentDateTime;
        //    var isOptoClass = Any<bool>();
        //    var isFeeback = Any<bool>();
        //    var endDate = currentDateTime.AddDays(1);

        //    var methodCall = await Assert.ThrowsAsync<ValidationException>(() =>
        //        _machineDataAppService.GetProductionResults(startDate, isOptoClass, isFeeback, endDate, null));

        //    Assert.Equal("DifferentDays", methodCall.Message);
        //}
    }
}
