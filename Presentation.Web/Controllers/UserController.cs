using Domain.Interfaces.Services;
using Domain.Objects.Requests.User;
using Domain.Utils.Languages;
using FluentValidation;
using FluentValidation.Results;
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

                var result = await userService.Get(currentPage, userName);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return HandleException(ex, "UserNotFound");
            }
        }

        [HttpPost("Save"), AllowAnonymous]
        public async Task<IActionResult> RegisterUser(UserRequest userRequest)
        {
            try
            {
                ValidateRequest(userRequestValidator, userRequest);
                var result = await userService.Save(userRequest);
                return StatusCode(StatusCodes.Status201Created, result);
            }
            catch (Exception ex)
            {
                return HandleException(ex, "ErrorSaving");
            }
        }

        [HttpPost("LogIn"), AllowAnonymous]
        public async Task<IActionResult> LogIn(LogInRequest logInRequest)
        {
            try
            {
                ValidateRequest(logInRequestValidator, logInRequest);
                var result = await userService.LogIn(logInRequest);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return HandleException(ex, "ErrorLoggingIn");
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
                return HandleException(ex, "ErrorTest");
            }
        }

        private IActionResult HandleException(Exception ex, string defaultMessageKey)
        {
            var message = !ex.Message.IsNullOrEmpty() ? Translator.Translate(ex.Message) : Translator.Translate(defaultMessageKey);
            return BadRequest(message);
        }

        private void ValidateRequest<T>(IValidator<T> validator, T request)
        {
            ValidationResult result = validator.Validate(request);
            if (!result.IsValid)
            {
                var errorMessages = string.Join("; ", result.Errors.Select(e => e.ErrorMessage));
                throw new ValidationException(errorMessages);
            }
        }
    }
}
