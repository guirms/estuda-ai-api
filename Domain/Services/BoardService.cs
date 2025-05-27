using AutoMapper;
using Domain.Interfaces.Repositories;
using Domain.Interfaces.Services;
using Domain.Models;
using Domain.Objects.Requests.User;
using Domain.Objects.Responses.Asset;
using Domain.Utils.Helpers;

namespace Domain.Services
{
    public class BoardService(IMapper mapper, IBoardRepository boardRepository) : IBoardService
    {
        public async Task Delete(int boardId)
        {
            var userId = HttpContextHelper.GetUserId();
            await boardRepository.DeleteByUserId(boardId, userId);
        }

        public async Task<IEnumerable<BoardResultsResponse>?> Get(int currentPage, string? boardName)
        {
            var userId = HttpContextHelper.GetUserId();
            return await boardRepository.GetBoardResults(userId, currentPage, boardName);
        }

        public async Task Save(SaveBoardRequest request)
        {
            var userId = HttpContextHelper.GetUserId();

            if (await NomeDuplicado(request.Name, userId))
                throw new InvalidOperationException("Board com o mesmo nome já cadastrado");

            var board = mapper.Map<Board>(request);
            board.UserId = userId;

            await boardRepository.Save(board);
        }

        public async Task Update(UpdateBoardRequest request)
        {
            var userId = HttpContextHelper.GetUserId();

            var board = await boardRepository.GetByIdAndUserId(request.BoardId, userId)
                        ?? throw new InvalidOperationException("Board não encontrado");

            if (!string.IsNullOrWhiteSpace(request.Name) &&
                await NomeDuplicado(request.Name, userId, board.BoardId))
            {
                throw new InvalidOperationException("Board com o mesmo nome já cadastrado");
            }

            board = request.MapIgnoringNullProperties(board);
            board.UpdatedAt = DateTime.Now;

            await boardRepository.Update(board);
        }
        private async Task<bool> NomeDuplicado(string? name, int userId, int? excludeId = null)
        {
            return !string.IsNullOrWhiteSpace(name) &&
                   await boardRepository.HasBoardWithSameNameAndUserId(name, userId, excludeId);
        }
    }
}
