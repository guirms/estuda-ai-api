using Domain.Objects.Requests.User;
using Domain.Objects.Responses.Asset;

namespace Domain.Interfaces.Services
{
    public interface IBoardService
    {
        Task<string> Save(BoardRequest boardRequest);
    }
}
