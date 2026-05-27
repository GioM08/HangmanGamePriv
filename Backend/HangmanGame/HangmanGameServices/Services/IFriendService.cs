using System.ServiceModel;
using HangmanGameEntities.Dtos;

namespace HangmanGameServices.Services
{
    [ServiceContract]
    public interface IFriendService
    {
        [OperationContract]
        FriendOperationResultDto SendFriendRequest(SendFriendRequestDto requestDto);

        [OperationContract]
        FriendOperationResultDto GetPendingFriendRequests(int userId);

        [OperationContract]
        FriendOperationResultDto GetSentFriendRequests(int userId);

        [OperationContract]
        FriendOperationResultDto AcceptFriendRequest(RespondFriendRequestDto requestDto);

        [OperationContract]
        FriendOperationResultDto RejectFriendRequest(RespondFriendRequestDto requestDto);

        [OperationContract]
        FriendOperationResultDto CancelFriendRequest(RespondFriendRequestDto requestDto);

        [OperationContract]
        FriendOperationResultDto RemoveFriend(RemoveFriendDto requestDto);

        [OperationContract]
        FriendOperationResultDto GetFriends(int userId);

        [OperationContract]
        FriendOperationResultDto SendFriendRequestByEmail(SendFriendRequestByEmailDto requestDto);
    }
}
