using Domain.Interfaces.Repositories;
using Domain.Interfaces.Services;
using Domain.Objects.Requests.Card;
using Domain.Objects.Responses.Board;
using Domain.Utils.Helpers;

namespace Domain.Services
{
    public class CardService(ICardRepository cardRepository) : ICardService
    {
        public async Task<IEnumerable<GetCardsResponse>?> Get(int boardId)
        {
            var result = await cardRepository.GetCardResultsByBoardId(boardId, 1, null);

            if (result == null || !result.Any())
                throw new InvalidOperationException("Nenhum card encontrado");

            return AgruparCardsPorStatus(result);
        }

        public async Task UpdateStatus(UpdateCardStatusRequest[] updateRequests)
        {
            var userId = HttpContextHelper.GetUserId();
            var cardIds = updateRequests.Select(r => r.CardId);
            var cards = await cardRepository.GetByIdAndUserId(cardIds, userId);

            if (cards == null || !cards.Any())
                throw new InvalidOperationException("Cards não encontrados");

            AtualizarStatusDosCards(cards, updateRequests);
            await cardRepository.UpdateMany(cards);
        }

        // Métodos auxiliares
        private static IEnumerable<GetCardsResponse> AgruparCardsPorStatus(IEnumerable<GetCardsResponseItem> cards)
        {
            return Enum.GetValues(typeof(Models.Enums.Task.ECardStatus))
                       .Cast<Models.Enums.Task.ECardStatus>()
                       .Select(status => new GetCardsResponse
                       {
                           TaskStatus = status,
                           Card = cards.Where(c => c.TaskStatus == status)
                       });
        }

        private static void AtualizarStatusDosCards(IEnumerable<Models.Card> cards, UpdateCardStatusRequest[] updateRequests)
        {
            var currentDateTime = DateTime.Now;

            var requestDict = updateRequests.ToDictionary(r => r.CardId, r => r.NewCardStatus);

            foreach (var card in cards)
            {
                if (requestDict.TryGetValue(card.CardId, out var newStatus))
                {
                    card.TaskStatus = newStatus;
                    card.UpdatedAt = currentDateTime;
                }
            }
        }
    }
}
