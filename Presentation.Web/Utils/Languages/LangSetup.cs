using Domain.Objects.Enums.Language;
using Domain.Utils.Languages;
using System.Reflection;
using System.Resources;

namespace Presentation.Web.Utils.Languages
{
    public static class LangSetup
    {
        public static void SetupLanguage(ELanguage currentLanguage)
        {
            Translator.CurrentLanguage = currentLanguage;

            var currentLanguageFilePath = Translator.CurrentLanguage switch
            {
                ELanguage.Portuguese => LangConfig.PortugueseLangFilePath,
                ELanguage.English => LangConfig.EnglishLangFilePath,
                ELanguage.Spanish => LangConfig.SpanishLangFilePath,
                ELanguage.Chinese => LangConfig.ChineseLangFilePath,
                _ => LangConfig.PortugueseLangFilePath
            };

            Translator.LanguageFile = new ResourceManager(currentLanguageFilePath, Assembly.GetExecutingAssembly());
        }
    }
}
