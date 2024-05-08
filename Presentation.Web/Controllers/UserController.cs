using Domain.Interfaces.Services;
using Domain.Objects.Requests.User;
using Domain.Utils.Languages;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Presentation.Web.Controllers
{
    [ApiController, Authorize, Route("User")]
    public class UserController(IUserService userService, IValidator<LogInRequest> logInRequestValidator, IValidator<UserRequest> userRequestValidator) : ControllerBase
    {
        [HttpGet("Get/{currentPage}")]
        public async Task<IActionResult> Get(int currentPage, string? userName)
        {
            try
            {
                if (currentPage < 1)
                    throw new InvalidOperationException("InvalidPage");

                return Ok(await userService.Get(currentPage, userName));
            }
            catch (Exception ex)
            {
                return BadRequest(!ex.Message.IsNullOrEmpty() ? Translator.Translate(ex.Message) : Translator.Translate("UserNotFound"));
            }
        }

        [HttpPost("Save"), AllowAnonymous]
        public async Task<IActionResult> Save(UserRequest userRequest)
        {
            try
            {
                userRequestValidator.Validate(userRequest);

                return new ObjectResult(await userService.Save(userRequest))
                {
                    StatusCode = StatusCodes.Status201Created
                };
            }
            catch (Exception ex)
            {
                return BadRequest(!ex.Message.IsNullOrEmpty() ? Translator.Translate(ex.Message) : Translator.Translate("ErrorSaving"));
            }
        }

        [HttpPost("LogIn"), AllowAnonymous]
        public async Task<IActionResult> LogIn(LogInRequest logInRequest)
        {
            try
            {
                logInRequestValidator.Validate(logInRequest);

                return Ok(await userService.LogIn(logInRequest));
            }
            catch (Exception ex)
            {
                return BadRequest(!ex.Message.IsNullOrEmpty() ? Translator.Translate(ex.Message) : Translator.Translate("ErrorLoggingIn"));
            }
        }

        [HttpGet("Test"), AllowAnonymous]
        public IActionResult Test()
        {
            try
            {
                return Ok("Ok!");
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro: {ex.Message}");
            }
        }
    }
}