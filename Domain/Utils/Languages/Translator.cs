using Domain.Objects.Enums.Language;
using System.Resources;

namespace Domain.Utils.Languages
{
    public static class Translator
    {
        public static ELanguage? CurrentLanguage { get; set; }
        public static ResourceManager? LanguageFile { get; set; }

        public static string Translate(string wordKey) => LanguageFile?.GetString(wordKey) ?? wordKey;
    }
}
