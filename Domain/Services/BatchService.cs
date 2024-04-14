using Domain.Interfaces.Repositories;
using Domain.Interfaces.Services;
using Domain.Models.Enums.Batch;
using Domain.Objects.Requests.Customer;
using Domain.Utils.Helpers;

namespace Domain.Services
{
    public class BatchService(ICustomerRepository customerRepository, IUserRepository userRepository) : IBatchService
    {
        public async Task Confirm(int customerId, DateTime insertedDateTime)
        {
            if (DateTime.Now.Subtract(insertedDateTime).TotalSeconds > 7)
                throw new InvalidOperationException("ErrorReceivingBatchConfirm");

            var customer = await customerRepository.GetById(customerId)
                ?? throw new InvalidOperationException("CustomerNotFound");

            await customerRepository.UpdateCustomerStatusToDisabled();

            customer.BatchStatus = EBatchStatus.Confirmed;

            await customerRepository.Update(customer);
        }

        public async Task<string> Start()
        {
            var confirmedCustomer = await customerRepository.GetConfirmedBatch()
                ?? throw new InvalidOperationException("CustomerNotFound");

            confirmedCustomer.BatchStatus = EBatchStatus.Started;

            await customerRepository.Update(confirmedCustomer);

            return confirmedCustomer.Name;
        }

        public async Task Stop(CustomerBaseInfoRequest customerBaseInfoRequest)
        {
            var customer = await customerRepository.GetById(customerBaseInfoRequest.CustomerId)
                ?? throw new InvalidOperationException("CustomerNotFound");

            customer.BatchStatus = EBatchStatus.Disabled;

            await customerRepository.Update(customer);
        }

        public async Task StopControl()
        {
            try
            {
                await userRepository.StartTransaction();
                await customerRepository.StartTransaction();

                var user = await userRepository.GetById(HttpContextHelper.GetUserId())
                    ?? throw new InvalidOperationException("UserNotFound");

                await userRepository.StartTransaction();

                var customersWithDisabledStatus = customerRepository.GetCustomersWithBatchStatusNotDisabled()
                    ?? throw new InvalidOperationException("CustomerNotFound");

                foreach (var customer in customersWithDisabledStatus)
                {
                    customer.BatchStatus = EBatchStatus.Disabled;
                }

                await customerRepository.UpdateMany(customersWithDisabledStatus);

                user.IsBatchDisabled = true;
                await userRepository.Update(user);

                await userRepository.CommitTransaction();
                await customerRepository.CommitTransaction();
            }
            catch
            {
                await userRepository.RollbackTransaction();
                await customerRepository.RollbackTransaction();

                throw new InvalidOperationException("ErrorStoppingBatchControl");
            }
        }

        public async Task StartControl()
        {
            var user = await userRepository.GetById(HttpContextHelper.GetUserId())
                ?? throw new InvalidOperationException("UserNotFound");

            user.IsBatchDisabled = false;

            await userRepository.Update(user);
        }

        public async Task ValidateConfirmed(int customerId)
        {
            var isConfirmed = await customerRepository.ValidateConfirmed(customerId);

            if (!isConfirmed)
                throw new InvalidOperationException("BatchNotConfirmed");
        }
    }
}
