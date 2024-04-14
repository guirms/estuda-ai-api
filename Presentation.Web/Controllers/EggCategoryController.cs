using Domain.Interfaces.Services;
using Domain.Objects.Requests.Egg;
using Domain.Utils.Languages;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Presentation.Web.Controllers
{
    [ApiController, Authorize, Route("EggCategory")]
    public class EggCategoryController(IEggCategoryService eggCategoryService, IValidator<IEnumerable<EggCategoriesRequest>> eggCategoriesRequestValidator) : ControllerBase
    {
        [HttpPatch("Update")]
        public async Task<IActionResult> Update(IEnumerable<EggCategoriesRequest> eggCategoriesRequest)
        {
            try
            {
                eggCategoriesRequestValidator.Validate(eggCategoriesRequest);

                await eggCategoryService.Update(eggCategoriesRequest);

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(!ex.Message.IsNullOrEmpty() ? Translator.Translate(ex.Message) : Translator.Translate("ErrorSaving"));
            }
        }
    }
}