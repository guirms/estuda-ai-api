using Domain.Interfaces.Repositories;
using Domain.Models;
using Infra.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace Infra.Data.Repositories
{
    public class BoardRepository(SqlContext context) : BaseSqlRepository<Board>(context), IBoardRepository
    {
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
