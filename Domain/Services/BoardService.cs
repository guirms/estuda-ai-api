using AutoMapper;
using Domain.Interfaces.Repositories;
using Domain.Interfaces.Services;
using Domain.Models;
using Domain.Objects.Requests.Card;
using Domain.Objects.Requests.User;
using Domain.Objects.Responses.Asset;
using Domain.Objects.Responses.Board;
using Domain.Utils.Helpers;

namespace Domain.Services
{
    public class BoardService(IMapper mapper, IBoardRepository boardRepository, ICardRepository cardRepository) : IBoardService
    {
        public async Task Delete(int boardId) => await boardRepository.DeleteByUserId(boardId, HttpContextHelper.GetUserId());

        public async Task<IEnumerable<BoardResultsResponse>?> Get(int currentPage, string? userName) =>
             await boardRepository.GetBoardResults(HttpContextHelper.GetUserId(), currentPage, userName);

        public async Task Save(SaveBoardRequest saveBoardRequest)
        {
            var userId = HttpContextHelper.GetUserId();

            if (await boardRepository.HasBoardWithSameNameAndUserId(saveBoardRequest.Name, userId))
                throw new InvalidOperationException("Board com o mesmo nome já cadastrado");

            var board = mapper.Map<Board>(saveBoardRequest);

            board.UserId = userId;

            await boardRepository.Save(board);
        }

        public async Task Update(UpdateBoardRequest updateBoardRequest)
        {
            var userId = HttpContextHelper.GetUserId();

            var board = await boardRepository.GetByIdAndUserId(updateBoardRequest.BoardId, HttpContextHelper.GetUserId())
                ?? throw new InvalidOperationException("Board não encontrado");

            if (updateBoardRequest.Name != null && await boardRepository.HasBoardWithSameNameAndUserId(updateBoardRequest.Name, userId, board.BoardId))
                throw new InvalidOperationException("Board com o mesmo nome já cadastrado");

            board = updateBoardRequest.MapIgnoringNullProperties(board);

            board.UpdatedAt = DateTime.Now;

            await boardRepository.Update(board);
        }

        public async Task<IEnumerable<CardResultsResponse>?> GetCards(int boardId)
        {
            var result = await cardRepository.GetCardResultsByBoardId(boardId, 1, null);

            if (result == null || !result.Any())
                throw new InvalidOperationException("Nenhum card encontrado");

            return result;
        }

        public async Task UpdateCardStatus(UpdateCardStatusRequest[] updateCardStatusRequest)
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
