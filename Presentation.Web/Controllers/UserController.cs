using Domain.Interfaces.Services;
using Domain.Objects.Requests.Customer;
using Domain.Objects.Requests.User;
using Domain.Utils.Helpers;
using Domain.Utils.Languages;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Presentation.Web.Controllers.Attributes;

namespace Presentation.Web.Controllers
{
    [ApiController, Authorize, Route("User")]
    public class UserControllerController(IUserService userService, IValidator<LogInRequest> logInRequestValidator, IValidator<UserRequest> userRequestValidator, IValidator<NewPasswordRequest> newPasswordRequestValidator, IValidator<PasswordRecoveryRequest> passwordRecoveryRequestValidator) : ControllerBase
    {
        [HttpGet("Get/{currentPage}"), IntraValidation]
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

                await userService.Save(userRequest);

                return StatusCode(StatusCodes.Status201Created);
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

        [HttpPost("IntraLogIn"), AllowAnonymous]
        public IActionResult IntraLogIn(IntraLogInRequest intraLogInRequest)
        {
            try
            {
                return Ok(userService.IntraLogIn(intraLogInRequest));
            }
            catch (Exception ex)
            {
                return BadRequest(!ex.Message.IsNullOrEmpty() ? Translator.Translate(ex.Message) : Translator.Translate("ErrorLoggingIn"));
            }
        }

        [HttpPost("LogOut")]
        public IActionResult LogOut()
        {
            try
            {
                userService.LogOut();

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(!ex.Message.IsNullOrEmpty() ? Translator.Translate(ex.Message) : Translator.Translate("ErrorLoggingOut"));
            }
        }

        [HttpGet("HasValidSession/{validateAssetFilter}")]
        public async Task<IActionResult> HasValidSession(bool validateAssetFilter)
        {
            try
            {
                return Ok(await userService.HasValidSession(validateAssetFilter));
            }
            catch (Exception ex)
            {
                return BadRequest(!ex.Message.IsNullOrEmpty() ? Translator.Translate(ex.Message) : Translator.Translate("InvalidAuthToken"));
            }
        }

        [HttpPost("SendPasswordRecovery"), AllowAnonymous]
        public async Task<IActionResult> SendPasswordRecovery(PasswordRecoveryRequest passwordRecoveryRequest)
        {
            try
            {
                passwordRecoveryRequestValidator.Validate(passwordRecoveryRequest);

                await userService.SendPasswordRecovery(passwordRecoveryRequest);

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(!ex.Message.IsNullOrEmpty() ? Translator.Translate(ex.Message) : Translator.Translate("ErrorResetingPassword"));
            }
        }

        [HttpPatch("RecoverPassword"), AllowAnonymous]
        public async Task<IActionResult> RecoverPassword(NewPasswordRequest newPasswordRequest)
        {
            try
            {
                newPasswordRequestValidator.Validate(newPasswordRequest);

                await userService.RecoverPassword(newPasswordRequest);

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(!ex.Message.IsNullOrEmpty() ? Translator.Translate(ex.Message) : Translator.Translate("ErrorResetingPassword"));
            }
        }

        [HttpGet("GenerateKey/{document}"), IntraValidation]
        public async Task<IActionResult> GenerateKey(string document)
        {
            try
            {
                if (!document.IsValidDocument())
                    throw new InvalidOperationException("InvalidDocument");

                return Ok(await userService.GenerateKey(document));
            }
            catch (Exception ex)
            {
                return BadRequest(!ex.Message.IsNullOrEmpty() ? Translator.Translate(ex.Message) : Translator.Translate("ErrorGeneratingUserKey"));
            }
        }

        [HttpPatch("UpdateAsset")]
        public async Task<IActionResult> UpdateAsset(UserAssetRequest userAssetRequest)
        {
            try
            {
                await userService.UpdateAsset(userAssetRequest);

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(!ex.Message.IsNullOrEmpty() ? Translator.Translate(ex.Message) : Translator.Translate("ErrorResetingPassword"));
            }
        }
    }
}