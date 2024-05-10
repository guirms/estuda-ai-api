using Domain.Models.Enums.Task;

namespace Domain.Objects.Requests.Card
{
    public record UpdateCardStatusRequest
    {
        public int CardId { get; init; }
        public ECardStatus NewCardStatus { get; init; }
    }
}
