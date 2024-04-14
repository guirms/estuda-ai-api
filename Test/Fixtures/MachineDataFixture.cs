using Application.Objects.Dto_s.Machine;
using Test.Setup;
using Xunit;

namespace Test.Fixtures
{
    [CollectionDefinition(nameof(MachineDataFixture))]
    public class MachineDataFixture : TestSetup
    {
        //protected readonly MachineDataService _machineDataService;
        //protected readonly MachineDataAppService _machineDataAppService;
        //protected readonly Mock<IMachineScheduleRepository> _machineScheduleRepository;
        protected readonly MachineScheduleDataDtoValidator _MachineScheduleDataDtoValidator = new();
        protected readonly MachineSchedulesDataDtoValidator _machineSchedulesDtoValidator = new();

        //internal MachineDataFixture()
        //{
        //    var eggResultsReportFile = new EggResultsReportFile(CreateMock<IHttpContextAccessor>().Object);

        //    _machineDataService = new MachineDataService(CreateMock<IMachineScheduleRepository>().Object, CreateMock<IMachineOperationRepository>().Object, CreateMock<IEggCategoryRepository>().Object, CreateMock<ICustomerRepository>().Object, CreateMock<IUserRepository>().Object, CreateMock<IValidator<IEnumerable<MachineScheduleDataDto>>>().Object, CreateMock<IValidator<MachineScheduleDataDto>>().Object, CreateMock<IConfiguration>().Object, CreateMock<IMapper>().Object, CreateMock<IHttpContextAccessor>().Object);
        //    _machineDataAppService = new MachineDataAppService(CreateMock<IMachineDataService>().Object, eggResultsReportFile);
        //    _machineScheduleRepository = CreateMock<IMachineScheduleRepository>();
        //}
    }
}
