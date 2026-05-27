using HangmanGameEntities.Dtos;

namespace HangmanGameBusiness.Friends
{
    public interface IFriendBusiness
    {
        FriendOperationResultDto SendFriendRequest(SendFriendRequestDto requestDto);

        FriendOperationResultDto GetPendingFriendRequests(int userId);

        FriendOperationResultDto GetSentFriendRequests(int userId);

        FriendOperationResultDto AcceptFriendRequest(RespondFriendRequestDto requestDto);

        FriendOperationResultDto RejectFriendRequest(RespondFriendRequestDto requestDto);

        FriendOperationResultDto CancelFriendRequest(RespondFriendRequestDto requestDto);

        FriendOperationResultDto RemoveFriend(RemoveFriendDto requestDto);

        FriendOperationResultDto GetFriends(int userId);

        FriendOperationResultDto SendFriendRequestByEmail(SendFriendRequestByEmailDto requestDto);
    }
}
