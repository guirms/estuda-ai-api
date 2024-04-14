using Test.Fixtures;

namespace Test.Tests.Services
{
    public class MachineDataServiceTest : MachineDataFixture
    {
        //private const string TraitName = "MachineDataService";
        //private const string GetProductionResults = "GetProductionResults";
        //private const string Save = "Save";

        //[Fact(DisplayName = "Empty machine schedule from database")]
        //[Trait(TraitName, Save)]
        //internal async void EmptyMachineScheduleFromDatabase_ThrowNoMachineScheduleFound()
        //{
        //    var methodCall = await Assert.ThrowsAsync<InvalidOperationException>(() =>
        //        _machineDataService.Save(Any<MachineResultsRequest>()));

        //    Assert.Equal("NoMachineScheduleFound", methodCall.Message);
        //}

        //[Fact(DisplayName = "Empty machine operation from database")]
        //[Trait(TraitName, GetProductionResults)]
        //internal async void EmptyMachineOperationFromDatabase_ThrowNoMachineOperationFound()
        //{
        //    var currentDateTime = DateTime.Now;

        //    var startDate = currentDateTime.AddHours(-1);
        //    var isOptoClass = Any<bool>();
        //    var isFeeback = Any<bool>();
        //    var endDate = currentDateTime;

        //    var methodCall = await Assert.ThrowsAsync<InvalidOperationException>(() =>
        //        _machineDataService.GetProductionResults(startDate, isOptoClass, isFeeback, endDate, null));

        //    Assert.Equal("NoMachineOperationFound", methodCall.Message);
        //}
    }
}
