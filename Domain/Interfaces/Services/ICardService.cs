using Domain.Objects.Requests.Card;
using Domain.Objects.Responses.Board;

namespace Domain.Interfaces.Services
{
    public interface ICardService
    {
        Task<IEnumerable<CardResultsResponse>?> Get(int boardId);
        Task UpdateStatus(UpdateCardStatusRequest[] updateCardStatusRequest);
    }
}
