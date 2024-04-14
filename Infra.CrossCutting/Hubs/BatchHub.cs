using Domain.Interfaces.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace Infra.CrossCutting.Hubs
{
    public class BatchHub(IHubContext<BatchHub, IBatchHub> batchHub) : Hub<IBatchHub>, IBatchHub
    {
        private readonly IHubContext<BatchHub, IBatchHub> _batchHub = batchHub;

        public async Task OnCurrentBatchRunning(string currentBatchRunning) => await _batchHub.Clients.All.OnCurrentBatchRunning(currentBatchRunning);
    }
}
