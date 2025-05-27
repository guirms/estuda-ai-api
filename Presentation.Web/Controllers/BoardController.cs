using Domain.Interfaces.Services;
using Domain.Objects.Requests.User;
using Domain.Utils.Languages;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Presentation.Web.Controllers
{
    [ApiController, Authorize, Route("Board")]
    public class BoardController(IBoardService boardService, IValidator<SaveBoardRequest> saveBoardRequestValidator, IValidator<UpdateBoardRequest> updateBoardRequestValidator) : ControllerBase
    {
        [HttpGet("Get/{currentPage}")]
        public async Task<IActionResult> GetBoards(int currentPage, string? boardName)
        {
            try
            {
                if (currentPage < 1)
                    throw new InvalidOperationException("InvalidPage");

                var result = await boardService.Get(currentPage, boardName);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return HandleException(ex, "BoardNotFound");
            }
        }

        [HttpPost("Save")]
        public async Task<IActionResult> SaveBoard(SaveBoardRequest saveBoardRequest)
        {
            try
            {
                ValidateRequest(saveBoardRequestValidator, saveBoardRequest);
                await boardService.Save(saveBoardRequest);
                return StatusCode(StatusCodes.Status201Created);
            }
            catch (Exception ex)
            {
                return HandleException(ex, "ErrorSaving");
            }
        }

        [HttpPatch("Update")]
        public async Task<IActionResult> UpdateBoard(UpdateBoardRequest updateBoardRequest)
        {
            try
            {
                ValidateRequest(updateBoardRequestValidator, updateBoardRequest);
                await boardService.Update(updateBoardRequest);
                return Ok("Quadro atualizado com sucesso");
            }
            catch (Exception ex)
            {
                return HandleException(ex, "ErrorSaving");
            }
        }

        [HttpDelete("Delete/{boardId}")]
        public async Task<IActionResult> DeleteBoard(int boardId)
        {
            try
            {
                await boardService.Delete(boardId);
                return NoContent();
            }
            catch (Exception ex)
            {
                return HandleException(ex, "ErrorSaving");
            }
        }

        private IActionResult HandleException(Exception ex, string defaultMessageKey)
        {
            var message = !ex.Message.IsNullOrEmpty() ? Translator.Translate(ex.Message) : Translator.Translate(defaultMessageKey);
            return BadRequest(message);
        }

        private void ValidateRequest<T>(IValidator<T> validator, T request)
        {
            ValidationResult result = validator.Validate(request);
            if (!result.IsValid)
            {
                var errorMessages = string.Join("; ", result.Errors.Select(e => e.ErrorMessage));
                throw new ValidationException(errorMessages);
            }
        }
    }
}
