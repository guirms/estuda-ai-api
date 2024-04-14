using Domain.Interfaces.Services;
using Domain.Objects.Requests.Asset;
using Domain.Objects.Requests.Customer;
using Domain.Utils.Languages;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Presentation.Web.Controllers.Attributes;

namespace Presentation.Web.Controllers
{
    [ApiController, Authorize, Route("Asset")]
    public class AssetController(IAssetService assetService, IValidator<SaveAssetRequest> saveAssetRequestValidator, IValidator<UpdateAssetRequest> updateAssetRequestValidator) : ControllerBase
    {
        [HttpGet("GetToTable/{currentPage}"), IntraValidation]
        public async Task<IActionResult> GetToTable(int currentPage, string? assetName)
        {
            try
            {
                if (currentPage < 1)
                    throw new InvalidOperationException("InvalidPage");

                return Ok(await assetService.GetToTable(currentPage, assetName));
            }
            catch (Exception ex)
            {
                return BadRequest(!ex.Message.IsNullOrEmpty() ? Translator.Translate(ex.Message) : Translator.Translate("AssetNotFound"));
            }
        }

        [HttpGet("GetToFilter/{plantId}/{currentPage}")]
        public async Task<IActionResult> GetToFilter(int currentPage, int plantId, string? assetName)
        {
            try
            {
                if (currentPage < 1)
                    throw new InvalidOperationException("InvalidPage");

                return Ok(await assetService.GetToFilter(currentPage, plantId, assetName));
            }
            catch (Exception ex)
            {
                return BadRequest(!ex.Message.IsNullOrEmpty() ? Translator.Translate(ex.Message) : Translator.Translate("AssetNotFound"));
            }
        }

        [HttpPost("Save"), IntraValidation]
        public async Task<IActionResult> Save(SaveAssetRequest saveAssetRequest)
        {
            try
            {
                saveAssetRequestValidator.Validate(saveAssetRequest);

                await assetService.Save(saveAssetRequest);

                return StatusCode(StatusCodes.Status201Created);
            }
            catch (Exception ex)
            {
                return BadRequest(!ex.Message.IsNullOrEmpty() ? Translator.Translate(ex.Message) : Translator.Translate("ErrorSaving"));
            }
        }

        [HttpPut("Update"), IntraValidation]
        public async Task<IActionResult> Update(UpdateAssetRequest updateAssetRequest)
        {
            try
            {
                updateAssetRequestValidator.Validate(updateAssetRequest);

                await assetService.Update(updateAssetRequest);

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(!ex.Message.IsNullOrEmpty() ? Translator.Translate(ex.Message) : Translator.Translate("ErrorUpdating"));
            }
        }

        [HttpDelete("Delete/{assetId}"), IntraValidation]
        public async Task<IActionResult> Delete(int assetId)
        {
            try
            {
                await assetService.Delete(assetId);

                return StatusCode(StatusCodes.Status204NoContent);
            }
            catch (Exception ex)
            {
                return BadRequest(!ex.Message.IsNullOrEmpty() ? Translator.Translate(ex.Message) : Translator.Translate("ErrorDeleting"));
            }
        }

        [HttpPatch("GenerateKey"), IntraValidation]
        public async Task<IActionResult> GenerateKey(AssetKeyRequest assetKeyRequest)
        {
            try
            {
                return Ok(await assetService.GenerateKey(assetKeyRequest.AssetId));
            }
            catch (Exception ex)
            {
                return BadRequest(!ex.Message.IsNullOrEmpty() ? Translator.Translate(ex.Message) : Translator.Translate("ErrorGeneratingKey"));
            }
        }

        [HttpPatch("GetConfig")]
        public async Task<IActionResult> GetConfig()
        {
            try
            {
                return Ok(await assetService.GetConfig());
            }
            catch (Exception ex)
            {
                return BadRequest(!ex.Message.IsNullOrEmpty() ? Translator.Translate(ex.Message) : Translator.Translate("ErrorGeneratingKey"));
            }
        }

        [HttpPatch("DeleteAuthToken")]
        public async Task<IActionResult> DeleteAuthToken()
        {
            try
            {
                await assetService.DeleteAuthToken();

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(!ex.Message.IsNullOrEmpty() ? Translator.Translate(ex.Message) : Translator.Translate("ErrorDeleting"));
            }
        }
    }
}