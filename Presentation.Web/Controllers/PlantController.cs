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
    [ApiController, Authorize, Route("Plant")]
    public class PlantController(IPlantService plantService, IValidator<SavePlantRequest> savePlantRequestValidator, IValidator<UpdatePlantRequest> updatePlantRequestValidator) : ControllerBase
    {
        [HttpGet("GetToTable/{currentPage}"), IntraValidation]
        public async Task<IActionResult> GetToTable(int currentPage, string? plantName, string? plantCnpj)
        {
            try
            {
                if (currentPage < 1)
                    throw new InvalidOperationException("InvalidPage");

                return Ok(await plantService.GetToTable(currentPage, plantName, plantCnpj));
            }
            catch (Exception ex)
            {
                return BadRequest(!ex.Message.IsNullOrEmpty() ? Translator.Translate(ex.Message) : Translator.Translate("PlantNotFound"));
            }
        }

        [HttpGet("GetToFilter/{currentPage}")]
        public async Task<IActionResult> GetToFilter(int currentPage, string? plantName, string? plantCnpj)
        {
            try
            {
                if (currentPage < 1)
                    throw new InvalidOperationException("InvalidPage");

                return Ok(await plantService.GetToFilter(currentPage, plantName, plantCnpj));
            }
            catch (Exception ex)
            {
                return BadRequest(!ex.Message.IsNullOrEmpty() ? Translator.Translate(ex.Message) : Translator.Translate("PlantNotFound"));
            }
        }

        [HttpPost("Save"), IntraValidation]
        public async Task<IActionResult> Save(SavePlantRequest savePlantRequest)
        {
            try
            {
                savePlantRequestValidator.Validate(savePlantRequest);

                await plantService.Save(savePlantRequest);

                return StatusCode(StatusCodes.Status201Created);
            }
            catch (Exception ex)
            {
                return BadRequest(!ex.Message.IsNullOrEmpty() ? Translator.Translate(ex.Message) : Translator.Translate("ErrorSaving"));
            }
        }

        [HttpPut("Update"), IntraValidation]
        public async Task<IActionResult> Update(UpdatePlantRequest updatePlantRequest)
        {
            try
            {
                updatePlantRequestValidator.Validate(updatePlantRequest);

                await plantService.Update(updatePlantRequest);

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(!ex.Message.IsNullOrEmpty() ? Translator.Translate(ex.Message) : Translator.Translate("ErrorUpdating"));
            }
        }

        [HttpDelete("Delete/{plantId}"), IntraValidation]
        public async Task<IActionResult> Delete(int plantId)
        {
            try
            {
                await plantService.Delete(plantId);

                return StatusCode(StatusCodes.Status204NoContent);
            }
            catch (Exception ex)
            {
                return BadRequest(!ex.Message.IsNullOrEmpty() ? Translator.Translate(ex.Message) : Translator.Translate("ErrorDeleting"));
            }
        }

        [HttpGet("GetSchema/{plantId}/{AssetId}")]
        public async Task<IActionResult> GetSchema(int plantId, int assetId)
        {
            try
            {
                return Ok(await plantService.GetSchema(plantId, assetId));
            }
            catch (Exception ex)
            {
                return BadRequest(!ex.Message.IsNullOrEmpty() ? Translator.Translate(ex.Message) : Translator.Translate("ErrorGettingPlantSchema"));
            }
        }
    }
}