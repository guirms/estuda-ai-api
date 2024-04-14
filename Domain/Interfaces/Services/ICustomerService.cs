using Domain.Objects.Requests.Customer;
using Domain.Objects.Responses.Customer;

namespace Domain.Interfaces.Services
{
    public interface ICustomerService
    {
        Task<CustomerToTableResponse> GetToTable(int currentPage, string? customerName);
        Task<IEnumerable<CustomerToFilterResponse>?> GetToFilter(int currentPage, string? customerName);
        Task Save(CustomerRequest customerRequest);
        Task Delete(int customerId);
    }
}