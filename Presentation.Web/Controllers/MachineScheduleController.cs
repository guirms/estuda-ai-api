using Application.Interfaces;
using Domain.Models.Enums.Scheduling;
using Domain.Objects.Requests.Machine;
using Domain.Utils.Languages;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Presentation.Web.Controllers
{
    [ApiController, Authorize, Route("MachineSchedule")]
    public class MachineScheduleController(IMachineScheduleService machineScheduleService, IValidator<MachineScheduleRequest> machineScheduleRequestValidator) : ControllerBase
    {
        [HttpGet("Get/{weekDay}")]
        public async Task<IActionResult> Get(EWeekDay weekDay)
        {
            try
            {
                return Ok(await machineScheduleService.Get(weekDay));
            }
            catch (Exception ex)
            {
                return BadRequest(!ex.Message.IsNullOrEmpty() ? Translator.Translate(ex.Message) : Translator.Translate("ErrorGettingMachineSchedules"));
            }
        }

        [HttpPost("Update")]
        public async Task<IActionResult> Update(MachineScheduleRequest machineScheduleRequest)
        {
            try
            {
                machineScheduleRequestValidator.Validate(machineScheduleRequest);

                await machineScheduleService.Update(machineScheduleRequest);

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(!ex.Message.IsNullOrEmpty() ? Translator.Translate(ex.Message) : Translator.Translate("ErrorSaving"));
            }
        }
    }
}