using Domain.Models;

namespace Domain.Interfaces.Repositories
{
    public interface IBoardRepository : IBaseSqlRepository<Board>
    {
        Task<bool> HasBoardWithSameNameAndUserId(string name, int userId);
    }
}
