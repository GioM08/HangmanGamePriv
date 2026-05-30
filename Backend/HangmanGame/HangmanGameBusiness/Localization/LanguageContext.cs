using System.Threading;

namespace HangmanGameBusiness.Localization
{
    public static class LanguageContext
    {
        private const string DefaultLanguage = "es-MX";
        private static readonly AsyncLocal<string> CurrentLanguage = new AsyncLocal<string>();

        public static void SetLanguage(string languageCode)
        {
            CurrentLanguage.Value = string.IsNullOrWhiteSpace(languageCode)
                ? DefaultLanguage
                : languageCode;
        }

        public static string GetLanguage()
        {
            return string.IsNullOrWhiteSpace(CurrentLanguage.Value)
                ? DefaultLanguage
                : CurrentLanguage.Value;
        }
    }
}
