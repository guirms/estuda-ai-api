using Domain.Interfaces.Services;
using Domain.Objects.Requests.Customer;
using Domain.Utils.Languages;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Presentation.Web.Controllers
{
    [ApiController, Authorize, Route("Customer")]
    public class CustomerController(ICustomerService customerService, IValidator<CustomerRequest> customerRequestValidator) : ControllerBase
    {
        [HttpGet("GetToTable/{currentPage}")]
        public async Task<IActionResult> GetToTable(int currentPage, string? customerName)
        {
            try
            {
                if (currentPage < 1)
                    throw new InvalidOperationException("InvalidPage");

                return Ok(await customerService.GetToTable(currentPage, customerName));
            }
            catch (Exception ex)
            {
                return BadRequest(!ex.Message.IsNullOrEmpty() ? Translator.Translate(ex.Message) : Translator.Translate("NoCustomersFound"));
            }
        }

        [HttpGet("GetToFilter/{currentPage}")]
        public async Task<IActionResult> GetToFilter(int currentPage, string? customerName)
        {
            try
            {
                if (currentPage < 1)
                    throw new InvalidOperationException("InvalidPage");

                return Ok(await customerService.GetToFilter(currentPage, customerName));
            }
            catch (Exception ex)
            {
                return BadRequest(!ex.Message.IsNullOrEmpty() ? Translator.Translate(ex.Message) : Translator.Translate("NoCustomersFound"));
            }
        }

        [HttpPost("Save")]
        public async Task<IActionResult> Save(CustomerRequest customerRequest)
        {
            try
            {
                customerRequestValidator.Validate(customerRequest);

                await customerService.Save(customerRequest);

                return StatusCode(StatusCodes.Status201Created);
            }
            catch (Exception ex)
            {
                return BadRequest(!ex.Message.IsNullOrEmpty() ? Translator.Translate(ex.Message) : Translator.Translate("ErrorSaving"));
            }
        }

        [HttpDelete("Delete/{customerId}")]
        public async Task<IActionResult> Delete(int customerId)
        {
            try
            {
                await customerService.Delete(customerId);

                return StatusCode(StatusCodes.Status204NoContent);
            }
            catch (Exception ex)
            {
                return BadRequest(!ex.Message.IsNullOrEmpty() ? Translator.Translate(ex.Message) : Translator.Translate("ErrorDeleting"));
            }
        }
    }
}