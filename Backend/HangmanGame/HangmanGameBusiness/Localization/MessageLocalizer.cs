using System.Globalization;
using System.Resources;

namespace HangmanGameBusiness.Localization
{
    public static class MessageLocalizer
    {
        private static readonly ResourceManager ResourceManager =
            new ResourceManager(
                "HangmanGameBusiness.Resources.ServerMessages",
                typeof(MessageLocalizer).Assembly
            );

        public static string Get(string messageKey)
        {
            CultureInfo cultureInfo;

            try
            {
                cultureInfo = new CultureInfo(LanguageContext.GetLanguage());
            }
            catch
            {
                cultureInfo = new CultureInfo("es-MX");
            }

            string message = ResourceManager.GetString(messageKey, cultureInfo);

            if (string.IsNullOrWhiteSpace(message))
            {
                message = ResourceManager.GetString(messageKey, new CultureInfo("es-MX"));
            }

            return string.IsNullOrWhiteSpace(message) ? messageKey : message;
        }
    }
}
