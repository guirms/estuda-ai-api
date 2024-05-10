using Domain.Objects.Requests.User;
using Domain.Objects.Responses.Asset;

namespace Domain.Interfaces.Services
{
    public interface IBoardService
    {
        Task Delete(int boardId);
        Task<IEnumerable<BoardResultsResponse>?> Get(int currentPage, string? userName);
        Task Save(SaveBoardRequest saveBoardRequest);
        Task Update(UpdateBoardRequest updateBoardRequest);
    }
}
