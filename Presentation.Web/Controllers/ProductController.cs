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
    [ApiController, Authorize, Route("Product")]
    public class ProductController(IProductService productService, IValidator<SaveProductRequest> saveProductRequestValidator, IValidator<UpdateProductRequest> updateProductRequestValidator) : ControllerBase
    {
        [HttpGet("GetToTable/{currentPage}"), IntraValidation]
        public async Task<IActionResult> GetToTable(int currentPage, string? productName)
        {
            try
            {
                if (currentPage < 1)
                    throw new InvalidOperationException("InvalidPage");

                return Ok(await productService.GetToTable(currentPage, productName));
            }
            catch (Exception ex)
            {
                return BadRequest(!ex.Message.IsNullOrEmpty() ? Translator.Translate(ex.Message) : Translator.Translate("ProductNotFound"));
            }
        }

        [HttpPost("Save"), IntraValidation]
        public async Task<IActionResult> Save(SaveProductRequest saveProductRequest)
        {
            try
            {
                saveProductRequestValidator.Validate(saveProductRequest);

                await productService.Save(saveProductRequest);

                return StatusCode(StatusCodes.Status201Created);
            }
            catch (Exception ex)
            {
                return BadRequest(!ex.Message.IsNullOrEmpty() ? Translator.Translate(ex.Message) : Translator.Translate("ErrorSaving"));
            }
        }

        [HttpPut("Update"), IntraValidation]
        public async Task<IActionResult> Update(UpdateProductRequest updateProductRequest)
        {
            try
            {
                updateProductRequestValidator.Validate(updateProductRequest);

                await productService.Update(updateProductRequest);

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(!ex.Message.IsNullOrEmpty() ? Translator.Translate(ex.Message) : Translator.Translate("ErrorUpdating"));
            }
        }

        [HttpDelete("Delete/{productId}"), IntraValidation]
        public async Task<IActionResult> Delete(int productId)
        {
            try
            {
                await productService.Delete(productId);

                return StatusCode(StatusCodes.Status204NoContent);
            }
            catch (Exception ex)
            {
                return BadRequest(!ex.Message.IsNullOrEmpty() ? Translator.Translate(ex.Message) : Translator.Translate("ErrorDeleting"));
            }
        }
    }
}