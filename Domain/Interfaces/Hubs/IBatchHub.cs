namespace Domain.Interfaces.Hubs
{
    public interface IBatchHub
    {
        Task OnCurrentBatchRunning(string currentBatchRunning);
    }
}
