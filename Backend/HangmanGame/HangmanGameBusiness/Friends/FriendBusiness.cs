using System;
using HangmanGameData.Repositories;
using HangmanGameData.Repositories.Interfaces;
using HangmanGameEntities.Dtos;

namespace HangmanGameBusiness.Friends
{
    public class FriendBusiness : IFriendBusiness
    {
        private const int PendingStatus = 0;

        private readonly IFriendRepository friendRepository;

        public FriendBusiness()
        {
            this.friendRepository = new FriendRepository();
        }

        public FriendBusiness(IFriendRepository friendRepository)
        {
            if (friendRepository == null)
            {
                throw new ArgumentNullException(nameof(friendRepository));
            }

            this.friendRepository = friendRepository;
        }

        public FriendOperationResultDto SendFriendRequest(SendFriendRequestDto requestDto)
        {
            try
            {
                if (requestDto == null)
                {
                    return Fail("Friend request data is required.");
                }

                if (requestDto.SenderUserId <= 0)
                {
                    return Fail("Sender user id is not valid.");
                }

                if (requestDto.ReceiverUserId <= 0)
                {
                    return Fail("Receiver user id is not valid.");
                }

                if (requestDto.SenderUserId == requestDto.ReceiverUserId)
                {
                    return Fail("You cannot send a friend request to yourself.");
                }

                if (!friendRepository.UserExists(requestDto.SenderUserId))
                {
                    return Fail("Sender user was not found.");
                }

                if (!friendRepository.UserExists(requestDto.ReceiverUserId))
                {
                    return Fail("Receiver user was not found.");
                }

                if (friendRepository.ActiveFriendRelationExists(
                    requestDto.SenderUserId,
                    requestDto.ReceiverUserId))
                {
                    return Fail("A pending request or friendship already exists.");
                }

                FriendRequestDto createdRequest = friendRepository.SendFriendRequest(requestDto);

                return new FriendOperationResultDto
                {
                    Success = true,
                    Message = "Friend request sent successfully.",
                    FriendRequest = createdRequest
                };
            }
            catch (Exception ex)
            {
                return Fail("Error: " + ex.Message);
            }
        }

        public FriendOperationResultDto GetPendingFriendRequests(int userId)
        {
            try
            {
                if (userId <= 0)
                {
                    return Fail("User id is not valid.");
                }

                if (!friendRepository.UserExists(userId))
                {
                    return Fail("User was not found.");
                }

                return new FriendOperationResultDto
                {
                    Success = true,
                    Message = "Pending friend requests retrieved successfully.",
                    FriendRequests = friendRepository.GetPendingFriendRequests(userId)
                };
            }
            catch (Exception)
            {
                return Fail("An unexpected error occurred while getting pending friend requests.");
            }
        }

        public FriendOperationResultDto GetSentFriendRequests(int userId)
        {
            try
            {
                if (userId <= 0)
                {
                    return Fail("User id is not valid.");
                }

                if (!friendRepository.UserExists(userId))
                {
                    return Fail("User was not found.");
                }

                return new FriendOperationResultDto
                {
                    Success = true,
                    Message = "Sent friend requests retrieved successfully.",
                    FriendRequests = friendRepository.GetSentFriendRequests(userId)
                };
            }
            catch (Exception)
            {
                return Fail("An unexpected error occurred while getting sent friend requests.");
            }
        }

        public FriendOperationResultDto AcceptFriendRequest(RespondFriendRequestDto requestDto)
        {
            try
            {
                FriendOperationResultDto validationResult = ValidateResponseRequest(
                    requestDto,
                    mustBeReceiver: true);

                if (!validationResult.Success)
                {
                    return validationResult;
                }

                FriendRequestDto updatedRequest = friendRepository.AcceptFriendRequest(
                    requestDto.FriendRequestId);

                return new FriendOperationResultDto
                {
                    Success = true,
                    Message = "Friend request accepted successfully.",
                    FriendRequest = updatedRequest
                };
            }
            catch (Exception)
            {
                return Fail("An unexpected error occurred while accepting the friend request.");
            }
        }

        public FriendOperationResultDto RejectFriendRequest(RespondFriendRequestDto requestDto)
        {
            try
            {
                FriendOperationResultDto validationResult = ValidateResponseRequest(
                    requestDto,
                    mustBeReceiver: true);

                if (!validationResult.Success)
                {
                    return validationResult;
                }

                FriendRequestDto updatedRequest = friendRepository.RejectFriendRequest(
                    requestDto.FriendRequestId);

                return new FriendOperationResultDto
                {
                    Success = true,
                    Message = "Friend request rejected successfully.",
                    FriendRequest = updatedRequest
                };
            }
            catch (Exception)
            {
                return Fail("An unexpected error occurred while rejecting the friend request.");
            }
        }

        public FriendOperationResultDto CancelFriendRequest(RespondFriendRequestDto requestDto)
        {
            try
            {
                FriendOperationResultDto validationResult = ValidateResponseRequest(
                    requestDto,
                    mustBeReceiver: false);

                if (!validationResult.Success)
                {
                    return validationResult;
                }

                FriendRequestDto updatedRequest = friendRepository.CancelFriendRequest(
                    requestDto.FriendRequestId);

                return new FriendOperationResultDto
                {
                    Success = true,
                    Message = "Friend request cancelled successfully.",
                    FriendRequest = updatedRequest
                };
            }
            catch (Exception)
            {
                return Fail("An unexpected error occurred while cancelling the friend request.");
            }
        }

        public FriendOperationResultDto RemoveFriend(RemoveFriendDto requestDto)
        {
            try
            {
                if (requestDto == null)
                {
                    return Fail("Remove friend data is required.");
                }

                if (requestDto.CurrentUserId <= 0)
                {
                    return Fail("Current user id is not valid.");
                }

                if (requestDto.FriendUserId <= 0)
                {
                    return Fail("Friend user id is not valid.");
                }

                if (requestDto.CurrentUserId == requestDto.FriendUserId)
                {
                    return Fail("You cannot remove yourself as a friend.");
                }

                if (!friendRepository.UserExists(requestDto.CurrentUserId))
                {
                    return Fail("Current user was not found.");
                }

                if (!friendRepository.UserExists(requestDto.FriendUserId))
                {
                    return Fail("Friend user was not found.");
                }

                FriendRequestDto acceptedRelation = friendRepository.GetAcceptedFriendRelation(
                    requestDto.CurrentUserId,
                    requestDto.FriendUserId);

                if (acceptedRelation == null)
                {
                    return Fail("Friendship was not found.");
                }

                FriendRequestDto removedRelation = friendRepository.RemoveFriend(
                    acceptedRelation.FriendRequestId);

                return new FriendOperationResultDto
                {
                    Success = true,
                    Message = "Friend removed successfully.",
                    FriendRequest = removedRelation
                };
            }
            catch (Exception)
            {
                return Fail("An unexpected error occurred while removing the friend.");
            }
        }

        public FriendOperationResultDto GetFriends(int userId)
        {
            try
            {
                if (userId <= 0)
                {
                    return Fail("User id is not valid.");
                }

                if (!friendRepository.UserExists(userId))
                {
                    return Fail("User was not found.");
                }

                return new FriendOperationResultDto
                {
                    Success = true,
                    Message = "Friends retrieved successfully.",
                    Friends = friendRepository.GetFriends(userId)
                };
            }
            catch (Exception)
            {
                return Fail("An unexpected error occurred while getting friends.");
            }
        }

        private FriendOperationResultDto ValidateResponseRequest(
            RespondFriendRequestDto requestDto,
            bool mustBeReceiver)
        {
            if (requestDto == null)
            {
                return Fail("Friend request response data is required.");
            }

            if (requestDto.FriendRequestId <= 0)
            {
                return Fail("Friend request id is not valid.");
            }

            if (requestDto.CurrentUserId <= 0)
            {
                return Fail("Current user id is not valid.");
            }

            FriendRequestDto friendRequest = friendRepository.GetFriendRequestById(
                requestDto.FriendRequestId);

            if (friendRequest == null)
            {
                return Fail("Friend request was not found.");
            }

            if (friendRequest.Status != PendingStatus)
            {
                return Fail("Friend request is not pending.");
            }

            if (mustBeReceiver && friendRequest.ReceiverUserId != requestDto.CurrentUserId)
            {
                return Fail("Only the receiver can respond to this friend request.");
            }

            if (!mustBeReceiver && friendRequest.SenderUserId != requestDto.CurrentUserId)
            {
                return Fail("Only the sender can cancel this friend request.");
            }

            return new FriendOperationResultDto
            {
                Success = true,
                Message = "Validation successful.",
                FriendRequest = friendRequest
            };
        }

        private FriendOperationResultDto Fail(string message)
        {
            return new FriendOperationResultDto
            {
                Success = false,
                Message = message
            };
        }

        public FriendOperationResultDto SendFriendRequestByEmail(SendFriendRequestByEmailDto requestDto)
        {
            try
            {
                if (requestDto == null)
                {
                    return Fail("Friend request data is required.");
                }

                if (requestDto.SenderUserId <= 0)
                {
                    return Fail("Sender user id is not valid.");
                }

                if (string.IsNullOrWhiteSpace(requestDto.ReceiverEmail))
                {
                    return Fail("Receiver email is required.");
                }

                if (!friendRepository.UserExists(requestDto.SenderUserId))
                {
                    return Fail("Sender user was not found.");
                }

                int receiverUserId = friendRepository.GetUserIdByEmail(requestDto.ReceiverEmail);

                if (receiverUserId <= 0)
                {
                    return Fail("No user was found with that email.");
                }

                if (requestDto.SenderUserId == receiverUserId)
                {
                    return Fail("You cannot send a friend request to yourself.");
                }

                if (friendRepository.ActiveFriendRelationExists(requestDto.SenderUserId, receiverUserId))
                {
                    return Fail("A pending request or friendship already exists.");
                }

                SendFriendRequestDto sendFriendRequestDto = new SendFriendRequestDto
                {
                    SenderUserId = requestDto.SenderUserId,
                    ReceiverUserId = receiverUserId
                };

                FriendRequestDto createdRequest = friendRepository.SendFriendRequest(sendFriendRequestDto);

                return new FriendOperationResultDto
                {
                    Success = true,
                    Message = "Friend request sent successfully.",
                    FriendRequest = createdRequest
                };
            }
            catch (Exception)
            {
                return Fail("An unexpected error occurred while sending the friend request.");
            }
        }
    }
}
