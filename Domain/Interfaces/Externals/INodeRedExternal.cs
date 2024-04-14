using Domain.Objects.Requests.Batch;

namespace Domain.Interfaces.Externals
{
    public interface INodeRedExternal
    {
        Task Change(BatchRequest batchRequest);
    }
}
