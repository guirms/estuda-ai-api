using AutoMapper;
using Domain.Interfaces.Repositories;
using Domain.Models;
using Domain.Objects.Requests.User;
using Domain.Objects.Responses.Asset;
using Infra.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace Infra.Data.Repositories
{
    public class UserRepository(SqlContext context, IMapper mapper) : BaseSqlRepository<User>(context), IUserRepository
    {
        public async Task<User?> GetUserByEmail(string userEmail) => await _typedContext.AsNoTracking().FirstOrDefaultAsync(u => u.Email == userEmail);

        public async Task<bool> ValidateBatchStateById(int userId) => await _typedContext.AsNoTracking()
            .Where(u => u.UserId == userId).Select(u => u.IsBatchDisabled).FirstOrDefaultAsync();

        public async Task HasUserWithTheSameInfo(UserRequest userRequest, string document)
        {
            if (await HasUserWithSameEmail(userRequest.Email))
                throw new InvalidOperationException("UserWithSameEmailError");

            if (await HasUserWithSameDocument(document))
                throw new InvalidOperationException("UserWithSameDocumentError");

            if (await HasUserWithSameKey(userRequest.UserKey))
                throw new InvalidOperationException("UserWithSameKeyError");
        }

        public async Task<bool> IsFirstLogInOfDay(int userId) => await _typedContext.AsNoTracking().AnyAsync(u =>
            u.LastLogin.HasValue && u.LastLogin.Value.Date != DateTime.Now.Date);

        public async Task<int?> GetAssetIdById(int userId) => await _typedContext.AsNoTracking()
                .Where(u => u.UserId == userId).Select(u => u.AssetId).FirstOrDefaultAsync();

        public async Task<User?> GetUserIfHasAssetById(int userId, int assetId)
        {
            var query = await _typedContext
                .Include(u => u.Plants)!
                .ThenInclude(p => p.Assets)
                .FirstOrDefaultAsync(u => u.UserId == userId)
                    ?? throw new InvalidOperationException("UserNotFound");

            var hasAsset = query.Plants?
                .Any(p => p.Assets != null && p.Assets
                .Any(a => a.AssetId == assetId)) ?? false;

            if (!hasAsset)
                throw new InvalidOperationException("AssetNotFound");

            return query;
        }

        public async Task<bool> HasUserById(int userId) => await _typedContext.AsNoTracking().AnyAsync(u => u.UserId == userId);

        public async Task<bool> HasUserWithSameDocument(string document) => await _typedContext.AsNoTracking().AnyAsync(u => u.Document == document);

        public async Task<User?> GetUserByEmailAndRecoveryCode(string userEmail, string recoveryCode) =>
            await _typedContext.AsNoTracking().FirstOrDefaultAsync(u => u.Email == userEmail && u.RecoveryCode == recoveryCode);

        public async Task<IEnumerable<UserResultsResponse>?> GetUserResults(int currentPage, string? userName, int takeQuantity = 10)
        {
            var query = _typedContext
                    .AsNoTracking()
                    .OrderByDescending(u => u.UserId)
                    .Skip((currentPage - 1) * takeQuantity)
                    .Take(takeQuantity);

            if (userName != null)
                query = query.Where(u => u.Name.Contains(userName));

            return await mapper.ProjectTo<UserResultsResponse>(query).ToListAsync();
        }

        public async Task<bool> HasAssetById(int userId) => await _typedContext.AsNoTracking().Where(u => u.UserId == userId)
            .AnyAsync(u => u.AssetId.HasValue);

        private async Task<bool> HasUserWithSameEmail(string email) => await _typedContext.AsNoTracking().AnyAsync(u => u.Email == email);

        private async Task<bool> HasUserWithSameKey(string userKey) => await _typedContext.AsNoTracking().AnyAsync(u => u.Key == userKey);
    }
}
