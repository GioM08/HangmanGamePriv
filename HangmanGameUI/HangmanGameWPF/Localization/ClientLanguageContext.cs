using System;

namespace HangmanGameWPF.Localization
{
    public static class ClientLanguageContext
    {
        public const string DefaultLanguage = "es-MX";

        public static event EventHandler LanguageChanged;

        public static string CurrentLanguage { get; private set; } = DefaultLanguage;

        public static void SetLanguage(string languageCode)
        {
            string normalizedLanguage = string.IsNullOrWhiteSpace(languageCode)
                ? DefaultLanguage
                : languageCode;

            if (CurrentLanguage == normalizedLanguage)
            {
                return;
            }

            CurrentLanguage = normalizedLanguage;
            LanguageChanged?.Invoke(null, EventArgs.Empty);
        }
    }
}
