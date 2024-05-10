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
        [HttpGet("Get/{cardId}")]
        public async Task<IActionResult> Get(int cardId)
        {
            try
            {
                return Ok(await cardService.Get(cardId));
            }
            catch (Exception ex)
            {
                return BadRequest(!ex.Message.IsNullOrEmpty() ? Translator.Translate(ex.Message) : Translator.Translate("Erro buscando cards"));
            }
        }


        [HttpPatch("UpdateStatus")]
        public async Task<IActionResult> UpdateStatus(UpdateCardStatusRequest[] updateCardStatusRequest)
        {
            try
            {
                await cardService.UpdateStatus(updateCardStatusRequest);

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(!ex.Message.IsNullOrEmpty() ? Translator.Translate(ex.Message) : Translator.Translate("Erro ao atualizar status"));
            }
        }
    }
}