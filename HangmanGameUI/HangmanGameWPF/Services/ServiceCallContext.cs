using System;
using System.ServiceModel;
using HangmanGameWPF.Localization;

namespace HangmanGameWPF.Services
{
    public static class ServiceCallContext
    {
        private const string HeaderName = "LanguageCode";
        private const string HeaderNamespace = "urn:hangmangame";

        public static OperationContextScope CreateScope(object client)
        {
            IContextChannel contextChannel = client as IContextChannel;

            if (contextChannel == null)
            {
                throw new InvalidOperationException("Client is not a valid WCF context channel.");
            }

            OperationContextScope scope = new OperationContextScope(contextChannel);

            return scope;
        }
    }
}
