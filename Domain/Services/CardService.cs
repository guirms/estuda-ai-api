using Domain.Interfaces.Repositories;
using Domain.Interfaces.Services;
using Domain.Objects.Requests.Card;
using Domain.Objects.Responses.Board;
using Domain.Utils.Helpers;

namespace Domain.Services
{
    public class CardService(ICardRepository cardRepository) : ICardService
    {
        public async Task<IEnumerable<CardResultsResponse>?> Get(int boardId)
        {
            var result = await cardRepository.GetCardResultsByBoardId(boardId, 1, null);

            if (result == null || !result.Any())
                throw new InvalidOperationException("Nenhum card encontrado");

            return result;
        }

        public async Task UpdateStatus(UpdateCardStatusRequest[] updateCardStatusRequest)
        {
            var cardIds = updateCardStatusRequest.Select(c => c.CardId);

            var cards = await cardRepository.GetByIdAndUserId(cardIds, HttpContextHelper.GetUserId());

            if (cards == null || !cards.Any())
                throw new InvalidOperationException("Cards não encontrados");

            var currentDateTime = DateTime.Now;

            foreach (var card in cards)
            {
                foreach (var cardId in cardIds)
                {
                    if (card.CardId == cardId)
                    {
                        var newStatus = updateCardStatusRequest.First(c => c.CardId == cardId).NewCardStatus;

                        card.TaskStatus = newStatus;
                        card.UpdatedAt = currentDateTime;
                    }
                }
            }

            await cardRepository.UpdateMany(cards);
        }
    }
}
