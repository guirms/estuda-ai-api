using AutoMapper;
using Domain.Interfaces.Repositories;
using Domain.Models;
using Domain.Objects.Responses.Asset;
using Infra.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace Infra.Data.Repositories
{
    public class BoardRepository(SqlContext context, IMapper mapper) : BaseSqlRepository<Board>(context), IBoardRepository
    {
        public async Task<IEnumerable<BoardResultsResponse>?> GetBoardResults(int userId, int currentPage, string? boardName, int takeQuantity = 10)
        {
            var query = _typedContext
                    .AsNoTracking()
                    .Where(b => b.UserId == userId)
                    .OrderByDescending(b => b.BoardId)
                    .Skip((currentPage - 1) * takeQuantity)
                    .Take(takeQuantity);

            if (boardName != null)
                query = query.Where(b => b.Name.Contains(boardName));

            return await mapper.ProjectTo<BoardResultsResponse>(query).ToListAsync();
        }

        public async Task<Board?> GetByIdAndUserId(int boardId, int userId) =>
            await _typedContext.FirstOrDefaultAsync(b => b.BoardId == boardId && b.UserId == userId);

        public async Task<bool> HasBoardWithSameNameAndUserId(string name, int userId, int? boardId)
        {
            var query = _typedContext
                .AsNoTracking()
                .Where(b => b.Name == name && b.UserId == userId);

            if (boardId.HasValue)
                query = query.Where(b => b.BoardId != boardId);


            return await query.AnyAsync();
        }
    }
}
