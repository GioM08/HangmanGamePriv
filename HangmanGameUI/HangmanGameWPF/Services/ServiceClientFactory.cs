using System.ServiceModel;

namespace HangmanGameWPF.Services
{
    internal static class ServiceClientFactory
    {
        private const string BaseUrl = "http://localhost:62627/Services/";

        public static IUserService CreateUserClient()
        {
            var binding = new BasicHttpBinding
            {
                MaxReceivedMessageSize = 2147483647,
                ReaderQuotas = { MaxStringContentLength = 2147483647 }
            };
            var endpoint = new EndpointAddress(BaseUrl + "UserService.svc");
            return ChannelFactory<IUserService>.CreateChannel(binding, endpoint);
        }

        public static ICategoryService CreateCategoryClient()
        {
            var binding = new BasicHttpBinding
            {
                MaxReceivedMessageSize = 2147483647,
                ReaderQuotas = { MaxStringContentLength = 2147483647 }
            };
            var endpoint = new EndpointAddress(BaseUrl + "CategoryService.svc");
            return ChannelFactory<ICategoryService>.CreateChannel(binding, endpoint);
        }

        public static IGameService CreateGameClient(IGameCallback callbackInstance)
        {
            var binding = new WSDualHttpBinding
            {
                MaxReceivedMessageSize = 2147483647,
                Security = { Mode = WSDualHttpSecurityMode.None }
            };
            var endpoint = new EndpointAddress(BaseUrl + "GameService.svc");
            var context = new InstanceContext(callbackInstance);
            return new DuplexChannelFactory<IGameService>(context, binding, endpoint).CreateChannel();
        }

        public static void CloseChannel(object channel)
        {
            var comm = channel as ICommunicationObject;
            if (comm == null) return;
            try
            {
                if (comm.State == CommunicationState.Opened)
                    comm.Close();
                else
                    comm.Abort();
            }
            catch
            {
                comm.Abort();
            }
        }
    }
}
