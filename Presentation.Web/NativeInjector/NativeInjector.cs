using Domain.Interfaces.Externals;
using Domain.Interfaces.Repositories;
using Domain.Interfaces.Services;
using Domain.Objects.Requests.User;
using Domain.Services;
using FluentValidation;
using Infra.CrossCutting.Externals;
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

            #endregion

            #region Services

            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IEncryptionService, EncryptionService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IBoardService, BoardService>();

            #endregion

            #region Repositories

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IBoardRepository, BoardRepository>();

            #endregion

            #region Validators

            services.AddTransient<IValidator<UserRequest>, UserRequestValidator>();
            services.AddTransient<IValidator<LogInRequest>, LogInRequestValidator>();
            services.AddTransient<IValidator<SaveBoardRequest>, SaveBoardRequestValidator>();
            services.AddTransient<IValidator<UpdateBoardRequest>, UpdateBoardRequestValidator>();

            #endregion

            #region Externals

            services.AddHttpClient<NodeRedExternal>();
            services.AddTransient<INodeRedExternal, NodeRedExternal>();

            #endregion

            #region Configurations

            services.AddHttpContextAccessor();

            #endregion
        }
    }
}
