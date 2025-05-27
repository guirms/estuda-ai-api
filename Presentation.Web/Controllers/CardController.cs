using Domain.Interfaces.Services;
using Domain.Objects.Requests.Card;
using Domain.Utils.Languages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Presentation.Web.Controllers
{
    [ApiController, Authorize, Route("Card")]
    public class CardController(ICardService cardService) : ControllerBase
    {
        [HttpGet("Get/{boardId}")]
        public async Task<IActionResult> GetCardsByBoard(int boardId)
        {
            try
            {
                var cards = await cardService.Get(boardId);
                return Ok(cards);
            }
            catch (Exception ex)
            {
                return HandleException(ex, "Erro buscando cards");
            }
        }

        [HttpPatch("UpdateStatus")]
        public async Task<IActionResult> UpdateCardStatuses(UpdateCardStatusRequest[] updateCardStatusRequest)
        {
            try
            {
                await cardService.UpdateStatus(updateCardStatusRequest);
                return Ok("Status atualizado com sucesso");
            }
            catch (Exception ex)
            {
                return HandleException(ex, "Erro ao atualizar status");
            }
        }

        private IActionResult HandleException(Exception ex, string defaultMessage)
        {
            var message = !ex.Message.IsNullOrEmpty() ? Translator.Translate(ex.Message) : Translator.Translate(defaultMessage);
            return BadRequest(message);
        }
    }
}
