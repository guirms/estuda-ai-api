using Domain.Interfaces.Services;
using Domain.Objects.Requests.Customer;
using Domain.Utils.Languages;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Presentation.Web.Controllers.Attributes;

namespace Presentation.Web.Controllers
{
    [ApiController, Authorize, Route("Layout")]
    public class LayoutController(ILayoutService layoutService, IValidator<SaveLayoutRequest> saveLayoutRequestValidator, IValidator<UpdateLayoutRequest> updateLayoutRequestValidator) : ControllerBase
    {
        [HttpGet("GetToTable/{currentPage}"), IntraValidation]
        public async Task<IActionResult> GetToTable(int currentPage, string? layoutName)
        {
            try
            {
                if (currentPage < 1)
                    throw new InvalidOperationException("InvalidPage");

                return Ok(await layoutService.GetToTable(currentPage, layoutName));
            }
            catch (Exception ex)
            {
                return BadRequest(!ex.Message.IsNullOrEmpty() ? Translator.Translate(ex.Message) : Translator.Translate("LayoutNotFound"));
            }
        }

        [HttpPost("Save"), IntraValidation]
        public async Task<IActionResult> Save(SaveLayoutRequest saveLayoutRequest)
        {
            try
            {
                saveLayoutRequestValidator.Validate(saveLayoutRequest);

                await layoutService.Save(saveLayoutRequest);

                return StatusCode(StatusCodes.Status201Created);
            }
            catch (Exception ex)
            {
                return BadRequest(!ex.Message.IsNullOrEmpty() ? Translator.Translate(ex.Message) : Translator.Translate("ErrorSaving"));
            }
        }

        [HttpPut("Update"), IntraValidation]
        public async Task<IActionResult> Update(UpdateLayoutRequest updateLayoutRequest)
        {
            try
            {
                updateLayoutRequestValidator.Validate(updateLayoutRequest);

                await layoutService.Update(updateLayoutRequest);

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(!ex.Message.IsNullOrEmpty() ? Translator.Translate(ex.Message) : Translator.Translate("ErrorUpdating"));
            }
        }

        [HttpDelete("Delete/{layoutId}"), IntraValidation]
        public async Task<IActionResult> Delete(int layoutId)
        {
            try
            {
                await layoutService.Delete(layoutId);

                return StatusCode(StatusCodes.Status204NoContent);
            }
            catch (Exception ex)
            {
                return BadRequest(!ex.Message.IsNullOrEmpty() ? Translator.Translate(ex.Message) : Translator.Translate("ErrorDeleting"));
            }
        }
    }
}