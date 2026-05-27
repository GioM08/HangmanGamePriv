using HangmanGameBusiness.Friends;
using HangmanGameEntities.Dtos;

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
            return friendBusiness.SendFriendRequest(requestDto);
        }

        public FriendOperationResultDto GetPendingFriendRequests(int userId)
        {
            return friendBusiness.GetPendingFriendRequests(userId);
        }

        public FriendOperationResultDto GetSentFriendRequests(int userId)
        {
            return friendBusiness.GetSentFriendRequests(userId);
        }

        public FriendOperationResultDto AcceptFriendRequest(RespondFriendRequestDto requestDto)
        {
            return friendBusiness.AcceptFriendRequest(requestDto);
        }

        public FriendOperationResultDto RejectFriendRequest(RespondFriendRequestDto requestDto)
        {
            return friendBusiness.RejectFriendRequest(requestDto);
        }

        public FriendOperationResultDto CancelFriendRequest(RespondFriendRequestDto requestDto)
        {
            return friendBusiness.CancelFriendRequest(requestDto);
        }

        public FriendOperationResultDto RemoveFriend(RemoveFriendDto requestDto)
        {
            return friendBusiness.RemoveFriend(requestDto);
        }

        public FriendOperationResultDto GetFriends(int userId)
        {
            return friendBusiness.GetFriends(userId);
        }

        public FriendOperationResultDto SendFriendRequestByEmail(SendFriendRequestByEmailDto requestDto)
        {
            return friendBusiness.SendFriendRequestByEmail(requestDto);
        }
    }
}
