using Domain.Interfaces.Services;
using Domain.Objects.Requests.Card;
using Domain.Objects.Requests.User;
using Domain.Utils.Languages;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Presentation.Web.Controllers
{
    [ApiController, Authorize, Route("Board")]
    public class BoardController(IBoardService boardService, IValidator<SaveBoardRequest> saveBoardRequestValidator, IValidator<UpdateBoardRequest> updateBoardRequestValidator) : ControllerBase
    {
        [HttpGet("Get/{currentPage}")]
        public async Task<IActionResult> Get(int currentPage, string? boardName)
        {
            try
            {
                if (currentPage < 1)
                    throw new InvalidOperationException("InvalidPage");

                return Ok(await boardService.Get(currentPage, boardName));
            }
            catch (Exception ex)
            {
                return BadRequest(!ex.Message.IsNullOrEmpty() ? Translator.Translate(ex.Message) : Translator.Translate("BoardNotFound"));
            }
        }

        [HttpPost("Save")]
        public async Task<IActionResult> Save(SaveBoardRequest saveBoardRequest)
        {
            try
            {
                saveBoardRequestValidator.Validate(saveBoardRequest);

                await boardService.Save(saveBoardRequest);

                return StatusCode(StatusCodes.Status201Created);
            }
            catch (Exception ex)
            {
                return BadRequest(!ex.Message.IsNullOrEmpty() ? Translator.Translate(ex.Message) : Translator.Translate("ErrorSaving"));
            }
        }

        [HttpPatch("Update")]
        public async Task<IActionResult> Update(UpdateBoardRequest updateBoardRequest)
        {
            try
            {
                updateBoardRequestValidator.Validate(updateBoardRequest);

                await boardService.Update(updateBoardRequest);

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(!ex.Message.IsNullOrEmpty() ? Translator.Translate(ex.Message) : Translator.Translate("ErrorSaving"));
            }
        }

        [HttpDelete("Delete/{boardId}")]
        public async Task<IActionResult> Delete(int boardId)
        {
            try
            {
                await boardService.Delete(boardId);

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(!ex.Message.IsNullOrEmpty() ? Translator.Translate(ex.Message) : Translator.Translate("ErrorSaving"));
            }
        }

        [HttpGet("GetCards/{boardId}")]
        public async Task<IActionResult> GetCards(int boardId)
        {
            try
            {
                return Ok(await boardService.GetCards(boardId));
            }
            catch (Exception ex)
            {
                return BadRequest(!ex.Message.IsNullOrEmpty() ? Translator.Translate(ex.Message) : Translator.Translate("BoardNotFound"));
            }
        }


        [HttpPatch("UpdateCardStatus")]
        public async Task<IActionResult> UpdateCardStatus(UpdateCardStatusRequest[] updateCardStatusRequest)
        {
            try
            {
                await boardService.UpdateCardStatus(updateCardStatusRequest);

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(!ex.Message.IsNullOrEmpty() ? Translator.Translate(ex.Message) : Translator.Translate("ErrorSaving"));
            }
        }
    }
}