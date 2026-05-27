using System.ServiceModel;

namespace HangmanGameWPF.Services
{
    internal static class ServiceClientFactory
    {
        public static IUserService CreateUserClient()
        {
            return new ChannelFactory<IUserService>("BasicHttpBinding_IUserService")
                .CreateChannel();
        }

        public static ICategoryService CreateCategoryClient()
        {
            return new ChannelFactory<ICategoryService>("BasicHttpBinding_ICategoryService")
                .CreateChannel();
        }

        public static IFriendService CreateFriendClient()
        {
            return new ChannelFactory<IFriendService>("BasicHttpBinding_IFriendService")
                .CreateChannel();
        }

        public static IGameService CreateGameClient(IGameCallback callbackInstance)
        {
            var context = new InstanceContext(callbackInstance);

            return new DuplexChannelFactory<IGameService>(
                context,
                "WsDualHttpBinding_IGameService"
            ).CreateChannel();
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