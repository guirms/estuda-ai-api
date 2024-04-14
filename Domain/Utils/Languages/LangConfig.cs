using Domain.Objects.Enums.Language;
using Domain.Utils.Helpers;
using System.Globalization;

namespace Domain.Utils.Languages
{
    public static class LangConfig
    {
        #region Culture

        public static readonly CultureInfo CultureInfo = new(ELanguage.Portuguese.GetDescription());

        #endregion

        #region Codes

        public static readonly string EnglishUsLangCode = ELanguage.English.GetDescription();
        public static readonly string PortugueseBrLangCode = ELanguage.Portuguese.GetDescription();
        public static readonly string SpanishLangCode = ELanguage.Spanish.GetDescription();
        public static readonly string ChineseLangCode = ELanguage.Chinese.GetDescription();

        public static readonly string[] SupportedLanguages = [
            PortugueseBrLangCode,
            EnglishUsLangCode,
            SpanishLangCode,
            ChineseLangCode
        ];

        #endregion

        #region Paths

        public static readonly string PortugueseLangFilePath = $"Presentation.Web.Utils.Languages.{PortugueseBrLangCode}";
        public static readonly string EnglishLangFilePath = $"Presentation.Web.Utils.Languages.{EnglishUsLangCode}";
        public static readonly string SpanishLangFilePath = $"Presentation.Web.Utils.Languages.{SpanishLangCode}";
        public static readonly string ChineseLangFilePath = $"Presentation.Web.Utils.Languages.{ChineseLangCode}";

        #endregion

        #region Http headers 

        public const string LangHeaderName = "LangInfo";

        #endregion
    }
}
