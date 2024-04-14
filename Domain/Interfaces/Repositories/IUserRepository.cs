using Domain.Models;
using Domain.Objects.Requests.User;
using Domain.Objects.Responses.Asset;

namespace Domain.Interfaces.Repositories
{
    public interface IUserRepository : IBaseSqlRepository<User>
    {
        Task<User?> GetUserByEmail(string userEmail);
        Task<bool> ValidateBatchStateById(int userId);
        Task HasUserWithTheSameInfo(UserRequest userRequest, string document);
        Task<bool> IsFirstLogInOfDay(int userId);
        Task<int?> GetAssetIdById(int userId);
        Task<User?> GetUserIfHasAssetById(int userId, int assetId);
        Task<bool> HasUserById(int userId);
        Task<bool> HasUserWithSameDocument(string document);
        Task<User?> GetUserByEmailAndRecoveryCode(string userEmail, string recoveryCode);
        Task<IEnumerable<UserResultsResponse>?> GetUserResults(int currentPage, string? userName, int takeQuantity = 10);
        Task<bool> HasAssetById(int userId);
    }
}
