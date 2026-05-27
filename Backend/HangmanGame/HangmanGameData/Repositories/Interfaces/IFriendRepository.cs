using System.Collections.Generic;
using HangmanGameEntities.Dtos;

namespace HangmanGameData.Repositories.Interfaces
{
    public interface IFriendRepository
    {
        bool UserExists(int userId);

        bool ActiveFriendRelationExists(int firstUserId, int secondUserId);

        FriendRequestDto SendFriendRequest(SendFriendRequestDto requestDto);

        List<FriendRequestDto> GetPendingFriendRequests(int userId);

        int GetUserIdByEmail(string email);

        List<FriendRequestDto> GetSentFriendRequests(int userId);

        FriendRequestDto GetFriendRequestById(int friendRequestId);

        FriendRequestDto GetAcceptedFriendRelation(int firstUserId, int secondUserId);

        FriendRequestDto AcceptFriendRequest(int friendRequestId);

        FriendRequestDto RejectFriendRequest(int friendRequestId);

        FriendRequestDto CancelFriendRequest(int friendRequestId);

        FriendRequestDto RemoveFriend(int friendRequestId);

        List<FriendDto> GetFriends(int userId);
    }
}
