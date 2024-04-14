using Application.Interfaces;
using Domain.Models.Enums.Scheduling;
using Domain.Objects.Requests.Machine;
using Domain.Objects.Requests.Report;
using Domain.Objects.Responses.Base;
using Domain.Utils.Languages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Presentation.Web.Controllers
{
    [ApiController, Authorize, Route("MachineData")]
    public class MachineDataController(IMachineDataAppService machineDataAppService, IMachineDataService machineDataService) : ControllerBase
    {
        private readonly string[] _errorsToClearData = ["NoMachineOperationFound", "NoShiftFound", "ErrorApplyingFilters", "AssetNotFound", "InvalidAuthToken", "ErrorGettingSessionInfo"];

        [HttpPost("Save"), AllowAnonymous]
        public async Task<IActionResult> Save(MachineDataRequest machineDataRequest)
        {
            try
            {
                await machineDataService.Save(machineDataRequest);

                return StatusCode(StatusCodes.Status201Created);
            }
            catch (Exception ex)
            {
                return BadRequest(!ex.Message.IsNullOrEmpty() ? Translator.Translate(ex.Message) : Translator.Translate("ErrorSaving"));
            }
        }

        [HttpGet("GetProductionResults/{startDateTime}/{isOptoClass}/{isFeeback}/{isFiltered}")]
        public async Task<IActionResult> GetProductionResults(DateTime startDateTime, bool isOptoClass, bool isFeeback, bool isFiltered, EShiftType? shiftType, DateTime? endDateTime)
        {
            try
            {
                if (endDateTime.HasValue && endDateTime.Value < startDateTime)
                    throw new ValidationException("StartDateGreaterThanEndDate");

                if (endDateTime.HasValue && endDateTime.Value.Date != startDateTime.Date)
                    throw new ValidationException("DifferentDays");

                var machineResults = await machineDataService.GetProductionResults(startDateTime, isOptoClass, isFeeback, endDateTime, shiftType);
                machineResults.IsFiltered = isFiltered;

                return Ok(machineResults);
            }
            catch (Exception ex)
            {
                var returnMessage = !ex.Message.IsNullOrEmpty() ? Translator.Translate(ex.Message) : Translator.Translate("ErrorGettingProductionResults");

                return BadRequest(BaseResponse.ErrorObj(_errorsToClearData.Contains(ex.Message), returnMessage));
            }
        }

        [HttpGet("GetEggResults/{startDateTime}/{endDateTime}/{isOptoClass}/{isFiltered}")]
        public async Task<IActionResult> GetEggResults(DateTime startDateTime, DateTime endDateTime, bool isOptoClass, bool isFiltered, EShiftType? shiftType, string? customerIds)
        {
            try
            {
                if (endDateTime < startDateTime)
                    throw new ValidationException("StartDateGreaterThanEndDate");

                int[]? customerIdsConverted = null;

                if (customerIds != null)
                    customerIdsConverted = JsonConvert.DeserializeObject<int[]>(customerIds);

                var eggResults = await machineDataService.GetEggResults(startDateTime, endDateTime, isOptoClass, shiftType, customerIdsConverted);
                eggResults.IsFiltered = isFiltered;

                return Ok(eggResults);
            }
            catch (Exception ex)
            {
                var returnMessage = !ex.Message.IsNullOrEmpty() ? Translator.Translate(ex.Message) : Translator.Translate("ErrorGettingEggResults");

                var defaultEggCategories = await machineDataService.GetDefaultEggCategories(isOptoClass);

                return BadRequest(BaseResponse.ErrorObj(_errorsToClearData.Contains(ex.Message), returnMessage, defaultEggCategories));
            }
        }

        [HttpPost("GenerateEggResultsReportFile")]
        public IActionResult GenerateEggResultsReportFile(EggResultsReportFileRequest eggResultsReportFileRequest)
        {
            try
            {
                var eggResultsReport = machineDataAppService.GenerateEggResultsReportFile(eggResultsReportFileRequest);

                return File(eggResultsReport.ReportContent, eggResultsReport.ReportExtension);
            }
            catch
            {
                return BadRequest();
            }
        }
    }
}
