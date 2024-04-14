using Domain.Interfaces.Repositories;
using Infra.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Infra.Data.Repositories
{
    public class BaseSqlRepository<T> : IDisposable, IBaseSqlRepository<T> where T : class
    {
        protected readonly DbSet<T> _typedContext;
        private readonly SqlContext _context;
        private IDbContextTransaction? _transaction;

        protected BaseSqlRepository(SqlContext context)
        {
            _context = context;
            _typedContext = context.Set<T>();
        }

        public async Task<int> Save(T modelObject)
        {
            await _typedContext.AddAsync(modelObject);

            return await _context.SaveChangesAsync();
        }

        public async Task<int> Update(T modelObject)
        {
            _typedContext.Update(modelObject);

            return await _context.SaveChangesAsync();
        }

        public async Task<int> UpdateMany(IEnumerable<T> modelList)
        {
            _typedContext.UpdateRange(modelList);

            return await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<T>?> GetAll() => await _typedContext.ToListAsync();

        public async Task<T?> GetById(int id) => await _typedContext.FindAsync(id);

        public async Task Delete(int id)
        {
            var modelObject = await GetById(id)
                ?? throw new InvalidOperationException("RegisterNotFound");

            _typedContext.Remove(modelObject);

            await _context.SaveChangesAsync();
        }

        public async Task StartTransaction() => _transaction = await _context.Database.BeginTransactionAsync();

        public async Task CommitTransaction()
        {
            await _transaction!.CommitAsync();
            await _transaction!.DisposeAsync();
        }

        public async Task RollbackTransaction()
        {
            await _transaction!.RollbackAsync();
            await _transaction!.DisposeAsync();
        }

        public async void Dispose()
        {
            await _context.DisposeAsync();
            _transaction?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
