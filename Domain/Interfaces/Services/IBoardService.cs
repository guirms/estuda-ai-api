using Domain.Objects.Requests.User;

namespace Domain.Interfaces.Services
{
    public interface IBoardService
    {
        Task Save(SaveBoardRequest saveBoardRequest);
        Task Update(UpdateBoardRequest updateBoardRequest);
    }
}
