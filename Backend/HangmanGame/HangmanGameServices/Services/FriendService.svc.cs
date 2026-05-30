using HangmanGameBusiness.Friends;
using HangmanGameBusiness.Localization;
using HangmanGameEntities.Dtos;
using HangmanGameServices.Localization;

namespace HangmanGameServices.Services
{
    public class FriendService : IFriendService
    {
        private readonly IFriendBusiness friendBusiness;

        public FriendService()
        {
            this.friendBusiness = new FriendBusiness();
        }

        public FriendOperationResultDto SendFriendRequest(SendFriendRequestDto requestDto)
        {
            SetLanguage();
            return friendBusiness.SendFriendRequest(requestDto);
        }

        public FriendOperationResultDto GetPendingFriendRequests(int userId)
        {
            SetLanguage();
            return friendBusiness.GetPendingFriendRequests(userId);
        }

        public FriendOperationResultDto GetSentFriendRequests(int userId)
        {
            SetLanguage();
            return friendBusiness.GetSentFriendRequests(userId);
        }

        public FriendOperationResultDto AcceptFriendRequest(RespondFriendRequestDto requestDto)
        {
            SetLanguage();
            return friendBusiness.AcceptFriendRequest(requestDto);
        }

        public FriendOperationResultDto RejectFriendRequest(RespondFriendRequestDto requestDto)
        {
            SetLanguage();
            return friendBusiness.RejectFriendRequest(requestDto);
        }

        public FriendOperationResultDto CancelFriendRequest(RespondFriendRequestDto requestDto)
        {
            SetLanguage();
            return friendBusiness.CancelFriendRequest(requestDto);
        }

        public FriendOperationResultDto RemoveFriend(RemoveFriendDto requestDto)
        {
            SetLanguage();
            return friendBusiness.RemoveFriend(requestDto);
        }

        public FriendOperationResultDto GetFriends(int userId)
        {
            SetLanguage();
            return friendBusiness.GetFriends(userId);
        }

        public FriendOperationResultDto SendFriendRequestByEmail(SendFriendRequestByEmailDto requestDto)
        {
            SetLanguage();
            return friendBusiness.SendFriendRequestByEmail(requestDto);
        }

        private static void SetLanguage()
        {
            LanguageContext.SetLanguage(RequestLanguageReader.GetLanguageCode());
        }
    }
}
