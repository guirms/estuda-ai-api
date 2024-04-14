using Domain.Objects.Enums.Language;
using Domain.Utils.Helpers;
using Domain.Utils.Languages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Presentation.Web.Utils.Languages;

namespace Presentation.Web.Filters
{
    public class ActionFilter(IHttpContextAccessor contextAccessor) : ControllerBase, IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context)
        {
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            try
            {
                var langValue = contextAccessor.GetHeaderValue(LangConfig.LangHeaderName) ?? "pt-BR";

                if (langValue != null && (langValue.Length != 0 && LangConfig.SupportedLanguages.Contains(langValue) &&
                    Translator.CurrentLanguage?.GetDescription() != langValue || (Translator.CurrentLanguage.HasValue && Translator.LanguageFile == null)))
                {
                    Translator.CurrentLanguage = EnumHelper.GetEnumValueByDescription<ELanguage>(langValue);
                    LangSetup.SetupLanguage(Translator.CurrentLanguage ?? ELanguage.Portuguese);
                }
                else if (Translator.CurrentLanguage?.GetDescription() != langValue)
                    LangSetup.SetupLanguage(ELanguage.Portuguese);
            }
            catch
            {
                throw new FileNotFoundException("An unexpected error occurred during screen translation");
            }

            contextAccessor.SaveTokens();
        }
    }
}