using System.ServiceModel;
using System.ServiceModel.Channels;

namespace HangmanGameServices.Localization
{
    public static class RequestLanguageReader
    {
        private const string HeaderName = "LanguageCode";
        private const string HeaderNamespace = "urn:hangmangame";

        public static string GetLanguageCode()
        {
            try
            {
                MessageHeaders headers = OperationContext.Current == null
                    ? null
                    : OperationContext.Current.IncomingMessageHeaders;

                if (headers == null)
                {
                    return "es-MX";
                }

                int headerIndex = headers.FindHeader(HeaderName, HeaderNamespace);

                if (headerIndex < 0)
                {
                    return "es-MX";
                }

                string languageCode = headers.GetHeader<string>(headerIndex);

                return string.IsNullOrWhiteSpace(languageCode) ? "es-MX" : languageCode;
            }
            catch
            {
                return "es-MX";
            }
        }
    }
}
