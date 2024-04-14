using Domain.Utils.Helpers;
using Domain.Utils.Languages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Presentation.Web.Controllers.Attributes
{
    internal class IntraValidationAttribute() : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            try
            {
                HttpContextHelper.ValidatePermission();
            }
            catch (Exception ex)
            {
                filterContext.Result = new BadRequestObjectResult(Translator.Translate(ex.Message));
            }
        }
    }
}