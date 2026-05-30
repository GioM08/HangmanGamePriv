using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using HangmanGameWPF.Localization;

namespace HangmanGameWPF.Services
{
    internal sealed class LanguageEndpointBehavior : IEndpointBehavior
    {
        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        {
        }

        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
            clientRuntime.ClientMessageInspectors.Add(new LanguageMessageInspector());
        }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
        }

        public void Validate(ServiceEndpoint endpoint)
        {
        }
    }

    internal sealed class LanguageMessageInspector : IClientMessageInspector
    {
        private const string HeaderName = "LanguageCode";
        private const string HeaderNamespace = "urn:hangmangame";

        public void AfterReceiveReply(ref Message reply, object correlationState)
        {
        }

        public object BeforeSendRequest(ref Message request, System.ServiceModel.IClientChannel channel)
        {
            MessageHeader<string> languageHeader =
                new MessageHeader<string>(ClientLanguageContext.CurrentLanguage);

            request.Headers.Add(languageHeader.GetUntypedHeader(HeaderName, HeaderNamespace));

            return null;
        }
    }
}
