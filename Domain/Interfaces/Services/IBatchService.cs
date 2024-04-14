using Domain.Objects.Requests.Customer;

namespace Domain.Interfaces.Services
{
    public interface IBatchService
    {
        Task Confirm(int customerId, DateTime insertedDateTime);
        Task<string> Start();
        Task Stop(CustomerBaseInfoRequest customerBaseInfoRequest);
        Task StopControl();
        Task StartControl();
        Task ValidateConfirmed(int customerId);
    }
}
