using AutoMapper;
using Domain.Interfaces.Repositories;
using Domain.Interfaces.Services;
using Domain.Models;
using Domain.Objects.Requests.User;
using Domain.Objects.Responses.Asset;
using Domain.Objects.Responses.Board;
using Domain.Utils.Helpers;

namespace Domain.Services
{
    public class BoardService(IMapper mapper, IBoardRepository boardRepository) : IBoardService
    {
        public async Task Delete(int boardId) => await boardRepository.Delete(boardId);

        public async Task<IEnumerable<BoardResultsResponse>?> Get(int currentPage, string? userName) =>
             await boardRepository.GetBoardResults(currentPage, userName);

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

        public async Task<CardResultsResponse?> GetCards(int boardId)
        {
            var response = new CardResultsResponse
            {
                ToDo =
                [
                    new() {
                        CardId = 1,
                        Name = "Primeira guerra mundial",
                        Description = "Principais causas e consequências do conflito",
                        Order = 0,
                        StudyTime = "0d 3h 30min"
                    }
                ],
                Doing =
                [
                    new() {
                        CardId = 2,
                        Name = "Seguunda guerra mundial",
                        Description = "Principais causas e consequências do conflito",
                        Order = 0,
                        StudyTime = "0d 4h 0min"
                    }
                ],

                Done =
                [
                    new() {
                        CardId = 3,
                        Name = "Guerra fria",
                        Description = "Impactos da guerra fria na vida dos civis e militares",
                        Order = 0,
                        StudyTime = "0d 1h 45min"
                    }
                ]
            };

            return await Task.FromResult(response);
        }
    }
}
