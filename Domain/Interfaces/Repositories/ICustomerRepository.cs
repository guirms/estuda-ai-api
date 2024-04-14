using Domain.Models;

namespace Domain.Interfaces.Repositories
{
    public interface ICustomerRepository : IBaseSqlRepository<Customer>
    {
        Task<IEnumerable<T>?> GetCustomerTypedData<T>(int currentPage, string? customerNam, int takeQuantity = 10);
        Task<bool> HasCustomerWithTheSameName(string name);
        Task UpdateCustomerStatusToDisabled();
        Task<Customer?> GetConfirmedBatch();
        Task<int?> GetStartedCustomerIdById();
        Task<bool> ValidateConfirmed(int customerId);
        Task<string?> GetStartedCustomerName();
        IEnumerable<Customer>? GetCustomersWithBatchStatusNotDisabled();
    }
}
