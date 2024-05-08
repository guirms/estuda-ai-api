using Domain.Models;

namespace Domain.Interfaces.Repositories
{
    public interface IBoardRepository : IBaseSqlRepository<Board>
    {
        Task<Board?> GetByIdAndUserId(int boardId, int userId);
        Task<bool> HasBoardWithSameNameAndUserId(string name, int userId, int? boardId = null);
    }
}
