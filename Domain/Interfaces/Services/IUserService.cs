using Domain.Objects.Requests.User;
using Domain.Objects.Responses.Asset;
using Domain.Objects.Responses.Machine;

namespace Domain.Interfaces.Services
{
    public interface IUserService
    {
        Task<IEnumerable<UserResultsResponse>?> Get(int currentPage, string? userName);
        Task Save(UserRequest userRequest);
        Task<LogInResponse> LogIn(LogInRequest logInRequest);
        LogInResponse IntraLogIn(IntraLogInRequest intraLogInRequest);
        void LogOut();
        Task<UserSessionResponse> HasValidSession(bool validateAssetFilter);
        Task SendPasswordRecovery(PasswordRecoveryRequest passwordRecoveryRequest);
        Task RecoverPassword(NewPasswordRequest newPasswordRequest);
        Task<UserKeyResponse> GenerateKey(string document);
        Task UpdateAsset(UserAssetRequest userAssetRequest);
    }
}
