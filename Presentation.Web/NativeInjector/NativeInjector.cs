using Application.AppServices;
using Application.Interfaces;
using Application.Objects.Dto_s.Machine;
using Application.Reports.ReportFiles.EggResultsReport;
using Domain.Interfaces.Externals;
using Domain.Interfaces.Hubs;
using Domain.Interfaces.Mails;
using Domain.Interfaces.Repositories;
using Domain.Interfaces.Services;
using Domain.Objects.Requests.Customer;
using Domain.Objects.Requests.Egg;
using Domain.Objects.Requests.Machine;
using Domain.Objects.Requests.User;
using Domain.Services;
using FluentValidation;
using Infra.CrossCutting.Externals;
using Infra.CrossCutting.Hubs;
using Infra.CrossCutting.Mails;
using Infra.CrossCutting.Security;
using Infra.Data.Repositories;

namespace Presentation.Web.NativeInjector
{
    public class NativeInjector
    {
        public static void RegisterServices(IServiceCollection services)
        {
            #region Report files

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddTransient<EggResultsReportFile>();

            #endregion

            #region App services

            services.AddScoped<IMachineDataAppService, MachineDataAppService>();

            #endregion

            #region Services

            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IMachineDataService, MachineDataService>();
            services.AddScoped<IEggCategoryService, EggCategoryService>();
            services.AddScoped<IMachineScheduleService, MachineScheduleService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ICustomerService, CustomerService>();
            services.AddScoped<IEncryptionService, EncryptionService>();
            services.AddScoped<IBatchService, BatchService>();
            services.AddScoped<IAssetService, AssetService>();
            services.AddScoped<IPlantService, PlantService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<ILayoutService, LayoutService>();

            #endregion

            #region Repositories

            services.AddScoped<IMachineOperationRepository, MachineOperationRepository>();
            services.AddScoped<IMachineScheduleRepository, MachineScheduleRepository>();
            services.AddScoped<IEggCategoryRepository, EggCategoryRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<IAssetRepository, AssetRepository>();
            services.AddScoped<IPlantRepository, PlantRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<ILayoutRepository, LayoutRepository>();

            #endregion

            #region Validators

            services.AddTransient<IValidator<MachineScheduleRequest>, MachineScheduleRequestValidator>();
            services.AddTransient<IValidator<MachineScheduleDataDto>, MachineScheduleDataDtoValidator>();
            services.AddTransient<IValidator<UserRequest>, UserRequestValidator>();
            services.AddTransient<IValidator<LogInRequest>, LogInRequestValidator>();
            services.AddTransient<IValidator<CustomerRequest>, CustomerRequestValidator>();
            services.AddTransient<IValidator<NewPasswordRequest>, NewPasswordRequestValidator>();
            services.AddTransient<IValidator<SaveAssetRequest>, SaveAssetRequestValidator>();
            services.AddTransient<IValidator<SavePlantRequest>, SavePlantRequestValidator>();
            services.AddTransient<IValidator<UpdatePlantRequest>, UpdatePlantRequestValidator>();
            services.AddTransient<IValidator<UpdateAssetRequest>, UpdateAssetRequestValidator>();
            services.AddTransient<IValidator<PasswordRecoveryRequest>, PasswordRecoveryRequestValidator>();
            services.AddTransient<IValidator<SaveProductRequest>, SaveProductRequestValidator>();
            services.AddTransient<IValidator<UpdateProductRequest>, UpdateProductRequestValidator>();
            services.AddTransient<IValidator<SaveLayoutRequest>, SaveLayoutRequestValidator>();
            services.AddTransient<IValidator<UpdateLayoutRequest>, UpdateLayoutRequestValidator>();
            services.AddTransient<IValidator<IEnumerable<EggCategoriesRequest>>, EggCategoriesRequestValidator>();
            services.AddTransient<IValidator<IEnumerable<MachineScheduleDataDto>>, MachineSchedulesDataDtoValidator>();

            #endregion

            #region Externals

            services.AddHttpClient<NodeRedExternal>();
            services.AddTransient<INodeRedExternal, NodeRedExternal>();
            services.AddTransient<IPFLicSrvExternal, PFLicSrvExternal>();
            services.AddTransient<ITotvsExternal, TotvsExternal>();

            #endregion

            #region Hubs

            services.AddSingleton<IBatchHub, BatchHub>();

            #endregion

            #region Mails

            services.AddTransient<IMailerService, MailerService>();

            #endregion

            #region Configurations

            services.AddHttpContextAccessor();

            #endregion
        }
    }
}
