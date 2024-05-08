using Domain.Interfaces.Repositories;
using Domain.Models;
using Infra.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace Infra.Data.Repositories
{
    public class BoardRepository(SqlContext context) : BaseSqlRepository<Board>(context), IBoardRepository
    {
        public async Task<bool> HasBoardWithSameNameAndUserId(string name, int userId) =>
            await _typedContext.AsNoTracking().AnyAsync(b => b.Name == name && b.UserId == userId);
    }
}
