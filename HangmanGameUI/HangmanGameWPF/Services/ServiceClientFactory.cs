using System.ServiceModel;

namespace HangmanGameWPF.Services
{
    internal static class ServiceClientFactory
    {
        public static IUserService CreateUserClient()
        {
            var factory = new ChannelFactory<IUserService>("BasicHttpBinding_IUserService");
            AddLanguageHeader(factory);
            return factory.CreateChannel();
        }

        public static ICategoryService CreateCategoryClient()
        {
            var factory = new ChannelFactory<ICategoryService>("BasicHttpBinding_ICategoryService");
            AddLanguageHeader(factory);
            return factory.CreateChannel();
        }

        public static IFriendService CreateFriendClient()
        {
            var factory = new ChannelFactory<IFriendService>("BasicHttpBinding_IFriendService");
            AddLanguageHeader(factory);
            return factory.CreateChannel();
        }

        public static ILeaderboardService CreateLeaderboardClient()
        {
            var factory = new ChannelFactory<ILeaderboardService>("BasicHttpBinding_ILeaderboardService");
            AddLanguageHeader(factory);
            return factory.CreateChannel();
        }

        public static IAccountRecoveryService CreateAccountRecoveryClient()
        {
            var factory = new ChannelFactory<IAccountRecoveryService>("BasicHttpBinding_IAccountRecoveryService");
            AddLanguageHeader(factory);
            return factory.CreateChannel();
        }

        public static IGameService CreateGameClient(IGameCallback callbackInstance)
        {
            var context = new InstanceContext(callbackInstance);

            var factory = new DuplexChannelFactory<IGameService>(
                context,
                "WsDualHttpBinding_IGameService"
            );

            AddLanguageHeader(factory);

            return factory.CreateChannel();
        }

        private static void AddLanguageHeader<TContract>(ChannelFactory<TContract> factory)
        {
            factory.Endpoint.EndpointBehaviors.Add(new LanguageEndpointBehavior());
        }

        public static void CloseChannel(object channel)
        {
            var communicationObject = channel as ICommunicationObject;

            if (communicationObject == null)
            {
                return;
            }

            try
            {
                if (communicationObject.State == CommunicationState.Opened)
                {
                    communicationObject.Close();
                }
                else
                {
                    communicationObject.Abort();
                }
            }
            catch
            {
                communicationObject.Abort();
            }
        }
    }
}
