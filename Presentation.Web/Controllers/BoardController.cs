using Domain.Interfaces.Services;
using Domain.Objects.Requests.Board;
using Domain.Objects.Requests.User;
using Domain.Utils.Languages;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Presentation.Web.Controllers
{
    [ApiController, Authorize, Route("Board")]
    public class BoardController(IBoardService boardService, IValidator<BoardRequest> boardRequestValidator) : ControllerBase
    {
        //[HttpGet("Get/{currentPage}")]
        //public async Task<IActionResult> Get(int currentPage, string? boardName)
        //{
        //    try
        //    {
        //        if (currentPage < 1)
        //            throw new InvalidOperationException("InvalidPage");

        //        return Ok(await boardService.Get(currentPage, boardName));
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(!ex.Message.IsNullOrEmpty() ? Translator.Translate(ex.Message) : Translator.Translate("BoardNotFound"));
        //    }
        //}

        [HttpPost("Save"), AllowAnonymous]
        public async Task<IActionResult> Save(BoardRequest boardRequest)
        {
            try
            {
                boardRequestValidator.Validate(boardRequest);

                return new ObjectResult(await boardService.Save(boardRequest))
                {
                    StatusCode = StatusCodes.Status201Created
                };
            }
            catch (Exception ex)
            {
                return BadRequest(!ex.Message.IsNullOrEmpty() ? Translator.Translate(ex.Message) : Translator.Translate("ErrorSaving"));
            }
        }

        //[HttpPost("LogIn"), AllowAnonymous]
        //public async Task<IActionResult> LogIn(LogInRequest logInRequest)
        //{
        //    try
        //    {
        //        logInRequestValidator.Validate(logInRequest);

        //        return Ok(await boardService.LogIn(logInRequest));
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(!ex.Message.IsNullOrEmpty() ? Translator.Translate(ex.Message) : Translator.Translate("ErrorLoggingIn"));
        //    }
        //}

        //[HttpGet("Test"), AllowAnonymous]
        //public IActionResult Test()
        //{
        //    try
        //    {
        //        return Ok("Ok!");            
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest($"Erro: {ex.Message}");
        //    }
        //}
    }
}