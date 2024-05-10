using Domain.Models.Enums.Task;

namespace Domain.Objects.Responses.Board
{
    public record GetCardsResponse
    {
        public required ECardStatus TaskStatus { get; set; }
        public IEnumerable<CardResultsResponse>? Card { get; set; }
    }
}