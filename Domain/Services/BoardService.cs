using AutoMapper;
using Domain.Interfaces.Repositories;
using Domain.Interfaces.Services;
using Domain.Models;
using Domain.Objects.Requests.User;
using Domain.Utils.Helpers;

namespace Domain.Services
{
    public class BoardService(IMapper mapper, IBoardRepository boardRepository) : IBoardService
    {
        public async Task Save(BoardRequest boardRequest)
        {
            var userId = HttpContextHelper.GetUserId();

            if (await boardRepository.HasBoardWithSameNameAndUserId(boardRequest.Name, userId))
                throw new InvalidOperationException("Board com o mesmo nome já cadastrado");

            var board = mapper.Map<Board>(boardRequest);

            board.UserId = userId;
            board.InsertedAt = DateTime.Now;

            await boardRepository.Save(board);
        }
    }
}
