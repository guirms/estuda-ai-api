using Domain.Interfaces.Externals;
using Domain.Interfaces.Hubs;
using Domain.Interfaces.Services;
using Domain.Objects.Requests.Batch;
using Domain.Objects.Requests.Customer;
using Domain.Utils.Helpers;
using Domain.Utils.Languages;
using Infra.CrossCutting.Externals.Objects.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Presentation.Web.Controllers
{
    [ApiController, Authorize, Route("Batch")]
    public class BatchController(IBatchHub batchHub, IBatchService batchService, INodeRedExternal nodeRedExternal) : ControllerBase
    {
        [HttpPatch("Change")]
        public async Task<IActionResult> Change(BatchRequest batchRequest)
        {
            try
            {
                if (!batchRequest.Cnpj.IsValidCnpj())
                    throw new InvalidOperationException("InvalidCnpj");

                await nodeRedExternal.Change(batchRequest);

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(!ex.Message.IsNullOrEmpty() ? Translator.Translate(ex.Message) : Translator.Translate("ErrorConfirmingBatch"));
            }
        }

        [HttpPut("Confirm")]
        public async Task<IActionResult> Confirm(BatchNodeRedRequest batchNodeRedRequest)
        {
            try
            {
                if (!batchNodeRedRequest.Result)
                    throw new InvalidOperationException("ErrorReceivingBatchConfirm");

                await batchService.Confirm(batchNodeRedRequest.CustomerId.ToInt("CustomerNotFound"), DateTime.Parse(batchNodeRedRequest.InsertedDateTime));

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(!ex.Message.IsNullOrEmpty() ? Translator.Translate(ex.Message) : Translator.Translate("ErrorConfirmingBatch"));
            }
        }

        [HttpPut("Start")]
        public async Task<IActionResult> Start()
        {
            try
            {
                var currentBatchRunning = await batchService.Start()
                    ?? throw new InvalidOperationException();

                await batchHub.OnCurrentBatchRunning(currentBatchRunning);

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(!ex.Message.IsNullOrEmpty() ? Translator.Translate(ex.Message) : Translator.Translate("ErrorStartingBatch"));
            }
        }

        [HttpPatch("Stop")]
        public async Task<IActionResult> Stop(CustomerBaseInfoRequest customerBaseInfoRequest)
        {
            try
            {
                await batchService.Stop(customerBaseInfoRequest);

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(!ex.Message.IsNullOrEmpty() ? Translator.Translate(ex.Message) : Translator.Translate("ErrorStoppingBatchControl"));
            }
        }

        [HttpPatch("StopControl")]
        public async Task<IActionResult> StopControl()
        {
            try
            {
                await batchService.StopControl();

                return Ok(true);
            }
            catch (Exception ex)
            {
                return BadRequest(!ex.Message.IsNullOrEmpty() ? Translator.Translate(ex.Message) : Translator.Translate("ErrorStoppingBatchControl"));
            }
        }

        [HttpPatch("StartControl")]
        public async Task<IActionResult> StartControl()
        {
            try
            {
                await batchService.StartControl();

                return Ok(true);
            }
            catch (Exception ex)
            {
                return BadRequest(!ex.Message.IsNullOrEmpty() ? Translator.Translate(ex.Message) : Translator.Translate("ErrorConfirmingBatch"));
            }
        }

        [HttpGet("ValidateConfirmed/{customerId}")]
        public async Task<IActionResult> ValidateConfirmed(int customerId)
        {
            try
            {
                await batchService.ValidateConfirmed(customerId);

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(!ex.Message.IsNullOrEmpty() ? Translator.Translate(ex.Message) : Translator.Translate("ErrorConfirmingBatch"));
            }
        }
    }
}
