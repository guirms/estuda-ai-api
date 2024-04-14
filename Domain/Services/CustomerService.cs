using AutoMapper;
using Domain.Interfaces.Repositories;
using Domain.Interfaces.Services;
using Domain.Models;
using Domain.Objects.Requests.Customer;
using Domain.Objects.Responses.Customer;
using Domain.Utils.Helpers;

namespace Domain.Services
{
    public class CustomerService(ICustomerRepository customerRepository, IUserRepository userRepository, IMapper mapper) : ICustomerService
    {
        public async Task<CustomerToTableResponse> GetToTable(int currentPage, string? customerName)
        {
            var customersTableData = await customerRepository.GetCustomerTypedData<CustomerTableData>(currentPage, customerName, 20);

            var currentBatchRunning = customersTableData?.FirstOrDefault(c => c.IsSelected)?.Name
                ?? await customerRepository.GetStartedCustomerName();

            var isBatchDisabled = await userRepository.ValidateBatchStateById(HttpContextHelper.GetUserId());

            return new CustomerToTableResponse
            {
                CurrentBatchRunning = currentBatchRunning,
                CustomerTableData = customersTableData,
                IsBatchDisabled = isBatchDisabled
            };
        }

        public async Task<IEnumerable<CustomerToFilterResponse>?> GetToFilter(int currentPage, string? customerName) => await customerRepository.GetCustomerTypedData<CustomerToFilterResponse>(currentPage, customerName);

        public async Task Save(CustomerRequest customerRequest)
        {
            var hasCustomerWithTheSameName = await customerRepository.HasCustomerWithTheSameName(customerRequest.Name);

            if (hasCustomerWithTheSameName)
                throw new InvalidOperationException("CustomerWithTheSameNameError");

            await customerRepository.Save(mapper.Map<Customer>(customerRequest));
        }

        public async Task Delete(int customerId) => await customerRepository.Delete(customerId);
    }
}
