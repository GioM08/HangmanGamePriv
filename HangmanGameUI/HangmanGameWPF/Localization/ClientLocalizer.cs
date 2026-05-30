using System.Globalization;
using System.Resources;

namespace HangmanGameWPF.Localization
{
    public static class ClientLocalizer
    {
        private static readonly ResourceManager ResourceManager =
            new ResourceManager(
                "HangmanGameWPF.Resources.ClientMessages",
                typeof(ClientLocalizer).Assembly);

        public static string Get(string key)
        {
            CultureInfo culture;

            try
            {
                culture = new CultureInfo(ClientLanguageContext.CurrentLanguage);
            }
            catch
            {
                culture = new CultureInfo(ClientLanguageContext.DefaultLanguage);
            }

            string value = ResourceManager.GetString(key, culture);

            return string.IsNullOrWhiteSpace(value) ? key : value;
        }
    }
}
