using AutoMapper;
using Domain.Interfaces.Repositories;
using Domain.Models;
using Domain.Models.Enums.Batch;
using Infra.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace Infra.Data.Repositories
{
    public class CustomerRepository(SqlContext context, IMapper mapper) : BaseSqlRepository<Customer>(context), ICustomerRepository
    {
        public async Task<IEnumerable<T>?> GetCustomerTypedData<T>(int currentPage, string? customerName, int takeQuantity = 10)
        {
            if (customerName != null)
                return await mapper.ProjectTo<T>(_typedContext
                    .AsNoTracking()
                    .Where(c => c.Name.Contains(customerName, StringComparison.CurrentCultureIgnoreCase))
                    .OrderByDescending(c => c.CustomerId)
                    .Skip((currentPage - 1) * takeQuantity)
                    .Take(takeQuantity))
                    .ToListAsync();

            return await mapper.ProjectTo<T>(_typedContext
                    .AsNoTracking()
                    .Skip((currentPage - 1) * takeQuantity)
                    .OrderByDescending(c => c.CustomerId)
                    .Take(takeQuantity))
                    .ToListAsync();
        }

        public async Task<bool> HasCustomerWithTheSameName(string name) => await _typedContext.AsNoTracking().AnyAsync(c => c.Name == name);

        public async Task UpdateCustomerStatusToDisabled()
        {
            var customersWithDisabledStatus = GetCustomersWithBatchStatusNotDisabled()
                                ?? throw new InvalidOperationException("CustomerNotFound");

            foreach (var customer in customersWithDisabledStatus)
            {
                customer.BatchStatus = EBatchStatus.Disabled;
            }

            await UpdateMany(customersWithDisabledStatus);
        }

        public async Task<Customer?> GetConfirmedBatch() => await _typedContext.FirstOrDefaultAsync(c => c.BatchStatus == EBatchStatus.Confirmed);

        public async Task<int?> GetStartedCustomerIdById() => (await _typedContext.FirstOrDefaultAsync(c => c.CustomerId == 1))?.CustomerId;

        public async Task<bool> ValidateConfirmed(int customerId) =>
            await _typedContext.AnyAsync(c => c.BatchStatus == EBatchStatus.Confirmed && c.CustomerId == customerId);

        public async Task<string?> GetStartedCustomerName() => (await _typedContext.FirstOrDefaultAsync(c => c.BatchStatus == EBatchStatus.Started))?.Name;

        public IEnumerable<Customer>? GetCustomersWithBatchStatusNotDisabled() =>
            _typedContext.Where(c => c.BatchStatus != EBatchStatus.Disabled)
                ?? throw new InvalidOperationException("CustomerNotFound");
    }
}
