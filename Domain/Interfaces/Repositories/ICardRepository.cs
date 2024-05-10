using Domain.Models;
using Domain.Objects.Responses.Board;

namespace Domain.Interfaces.Repositories
{
    public interface ICardRepository : IBaseSqlRepository<Card>
    {
        Task<IEnumerable<Card>?> GetByIdAndUserId(IEnumerable<int> cardId, int userId);
        Task<IEnumerable<CardResultsResponse>?> GetCardResultsByBoardId(int boardId, int currentPage, string? cardName, int takeQuantity = 10);
    }
}
