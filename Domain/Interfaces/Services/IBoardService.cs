using Domain.Objects.Requests.User;

namespace Domain.Interfaces.Services
{
    public interface IBoardService
    {
        Task Save(BoardRequest boardRequest);
    }
}
