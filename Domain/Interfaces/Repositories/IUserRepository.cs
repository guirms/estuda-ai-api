using Domain.Models;
using Domain.Objects.Requests.User;
using Domain.Objects.Responses.Asset;

namespace Domain.Interfaces.Repositories
{
    public interface IUserRepository : IBaseSqlRepository<User>
    {
        Task<User?> GetUserByEmail(string userEmail);
        Task HasUserWithTheSameInfo(UserRequest userRequest, string document);
        Task<bool> HasUserWithSameDocument(string document);
        Task<IEnumerable<UserResultsResponse>?> GetUserResults(int currentPage, string? userName, int takeQuantity = 10);
    }
}
