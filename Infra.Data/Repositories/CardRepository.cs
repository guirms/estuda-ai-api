using AutoMapper;
using Domain.Interfaces.Repositories;
using Domain.Models;
using Domain.Objects.Responses.Asset;
using Domain.Objects.Responses.Board;
using Infra.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace Infra.Data.Repositories
{
    public class CardRepository(SqlContext context, IMapper mapper) : BaseSqlRepository<Card>(context), ICardRepository
    {
        public async Task<Card?> GetByIdAndUserId(int cardId, int userId) =>
            await _typedContext.Include(c => c.Board).FirstOrDefaultAsync(c => c.CardId == cardId && c.Board.UserId == userId);

        public async Task<IEnumerable<CardResultsResponse>?> GetCardResultsByBoardId(int boardId, int currentPage, string? cardName, int takeQuantity = 10)
        {
            var query = _typedContext
                    .AsNoTracking()
                    .Where(c => c.BoardId == boardId)
                    .OrderBy(c => c.TaskStatus)
                    .Skip((currentPage - 1) * takeQuantity)
                    .Take(takeQuantity);

            if (cardName != null)
                query = query.Where(b => b.Name.Contains(cardName));

            return await mapper.ProjectTo<CardResultsResponse>(query).ToListAsync();
        }
    }
}
