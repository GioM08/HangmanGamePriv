using System;
using HangmanGameBusiness.Localization;
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
                throw new ArgumentNullException("friendRepository");
            }

            this.friendRepository = friendRepository;
        }

        public FriendOperationResultDto SendFriendRequest(SendFriendRequestDto requestDto)
        {
            try
            {
                if (requestDto == null) return Fail(MessageKeys.FriendRequestDataRequired);
                if (requestDto.SenderUserId <= 0) return Fail(MessageKeys.SenderUserIdInvalid);
                if (requestDto.ReceiverUserId <= 0) return Fail(MessageKeys.ReceiverUserIdInvalid);
                if (requestDto.SenderUserId == requestDto.ReceiverUserId) return Fail(MessageKeys.CannotSendRequestToYourself);
                if (!friendRepository.UserExists(requestDto.SenderUserId)) return Fail(MessageKeys.SenderUserNotFound);
                if (!friendRepository.UserExists(requestDto.ReceiverUserId)) return Fail(MessageKeys.ReceiverUserNotFound);

                if (friendRepository.ActiveFriendRelationExists(requestDto.SenderUserId, requestDto.ReceiverUserId))
                {
                    return Fail(MessageKeys.FriendRequestAlreadyExists);
                }

                return Success(
                    MessageKeys.FriendRequestSentSuccessfully,
                    friendRequest: friendRepository.SendFriendRequest(requestDto));
            }
            catch (Exception)
            {
                return Fail(MessageKeys.UnexpectedError);
            }
        }

        public FriendOperationResultDto GetPendingFriendRequests(int userId)
        {
            try
            {
                if (userId <= 0) return Fail(MessageKeys.UserIdInvalid);
                if (!friendRepository.UserExists(userId)) return Fail(MessageKeys.UserNotFound);

                return Success(
                    MessageKeys.PendingFriendRequestsRetrievedSuccessfully,
                    friendRequests: friendRepository.GetPendingFriendRequests(userId));
            }
            catch (Exception)
            {
                return Fail(MessageKeys.UnexpectedError);
            }
        }

        public FriendOperationResultDto GetSentFriendRequests(int userId)
        {
            try
            {
                if (userId <= 0) return Fail(MessageKeys.UserIdInvalid);
                if (!friendRepository.UserExists(userId)) return Fail(MessageKeys.UserNotFound);

                return Success(
                    MessageKeys.SentFriendRequestsRetrievedSuccessfully,
                    friendRequests: friendRepository.GetSentFriendRequests(userId));
            }
            catch (Exception)
            {
                return Fail(MessageKeys.UnexpectedError);
            }
        }

        public FriendOperationResultDto AcceptFriendRequest(RespondFriendRequestDto requestDto)
        {
            try
            {
                FriendOperationResultDto validationResult = ValidateResponseRequest(requestDto, true);

                if (!validationResult.Success)
                {
                    return validationResult;
                }

                return Success(
                    MessageKeys.FriendRequestAcceptedSuccessfully,
                    friendRequest: friendRepository.AcceptFriendRequest(requestDto.FriendRequestId));
            }
            catch (Exception)
            {
                return Fail(MessageKeys.UnexpectedError);
            }
        }

        public FriendOperationResultDto RejectFriendRequest(RespondFriendRequestDto requestDto)
        {
            try
            {
                FriendOperationResultDto validationResult = ValidateResponseRequest(requestDto, true);

                if (!validationResult.Success)
                {
                    return validationResult;
                }

                return Success(
                    MessageKeys.FriendRequestRejectedSuccessfully,
                    friendRequest: friendRepository.RejectFriendRequest(requestDto.FriendRequestId));
            }
            catch (Exception)
            {
                return Fail(MessageKeys.UnexpectedError);
            }
        }

        public FriendOperationResultDto CancelFriendRequest(RespondFriendRequestDto requestDto)
        {
            try
            {
                FriendOperationResultDto validationResult = ValidateResponseRequest(requestDto, false);

                if (!validationResult.Success)
                {
                    return validationResult;
                }

                return Success(
                    MessageKeys.FriendRequestCancelledSuccessfully,
                    friendRequest: friendRepository.CancelFriendRequest(requestDto.FriendRequestId));
            }
            catch (Exception)
            {
                return Fail(MessageKeys.UnexpectedError);
            }
        }

        public FriendOperationResultDto RemoveFriend(RemoveFriendDto requestDto)
        {
            try
            {
                if (requestDto == null) return Fail(MessageKeys.RemoveFriendDataRequired);
                if (requestDto.CurrentUserId <= 0) return Fail(MessageKeys.CurrentUserIdInvalid);
                if (requestDto.FriendUserId <= 0) return Fail(MessageKeys.FriendUserIdInvalid);
                if (requestDto.CurrentUserId == requestDto.FriendUserId) return Fail(MessageKeys.CannotRemoveYourselfAsFriend);
                if (!friendRepository.UserExists(requestDto.CurrentUserId)) return Fail(MessageKeys.CurrentUserNotFound);
                if (!friendRepository.UserExists(requestDto.FriendUserId)) return Fail(MessageKeys.FriendUserNotFound);

                FriendRequestDto acceptedRelation = friendRepository.GetAcceptedFriendRelation(
                    requestDto.CurrentUserId,
                    requestDto.FriendUserId);

                if (acceptedRelation == null)
                {
                    return Fail(MessageKeys.FriendshipNotFound);
                }

                return Success(
                    MessageKeys.FriendRemovedSuccessfully,
                    friendRequest: friendRepository.RemoveFriend(acceptedRelation.FriendRequestId));
            }
            catch (Exception)
            {
                return Fail(MessageKeys.UnexpectedError);
            }
        }

        public FriendOperationResultDto GetFriends(int userId)
        {
            try
            {
                if (userId <= 0) return Fail(MessageKeys.UserIdInvalid);
                if (!friendRepository.UserExists(userId)) return Fail(MessageKeys.UserNotFound);

                return Success(
                    MessageKeys.FriendsRetrievedSuccessfully,
                    friends: friendRepository.GetFriends(userId));
            }
            catch (Exception)
            {
                return Fail(MessageKeys.UnexpectedError);
            }
        }

        public FriendOperationResultDto SendFriendRequestByEmail(SendFriendRequestByEmailDto requestDto)
        {
            try
            {
                if (requestDto == null) return Fail(MessageKeys.FriendRequestDataRequired);
                if (requestDto.SenderUserId <= 0) return Fail(MessageKeys.SenderUserIdInvalid);
                if (string.IsNullOrWhiteSpace(requestDto.ReceiverEmail)) return Fail(MessageKeys.ReceiverEmailRequired);
                if (!friendRepository.UserExists(requestDto.SenderUserId)) return Fail(MessageKeys.SenderUserNotFound);

                int receiverUserId = friendRepository.GetUserIdByEmail(requestDto.ReceiverEmail);

                if (receiverUserId <= 0) return Fail(MessageKeys.NoUserFoundWithEmail);
                if (requestDto.SenderUserId == receiverUserId) return Fail(MessageKeys.CannotSendRequestToYourself);
                if (friendRepository.ActiveFriendRelationExists(requestDto.SenderUserId, receiverUserId))
                {
                    return Fail(MessageKeys.FriendRequestAlreadyExists);
                }

                SendFriendRequestDto sendFriendRequestDto = new SendFriendRequestDto
                {
                    SenderUserId = requestDto.SenderUserId,
                    ReceiverUserId = receiverUserId
                };

                return Success(
                    MessageKeys.FriendRequestSentSuccessfully,
                    friendRequest: friendRepository.SendFriendRequest(sendFriendRequestDto));
            }
            catch (Exception)
            {
                return Fail(MessageKeys.UnexpectedError);
            }
        }

        private FriendOperationResultDto ValidateResponseRequest(
            RespondFriendRequestDto requestDto,
            bool mustBeReceiver)
        {
            if (requestDto == null) return Fail(MessageKeys.FriendRequestResponseDataRequired);
            if (requestDto.FriendRequestId <= 0) return Fail(MessageKeys.FriendRequestIdInvalid);
            if (requestDto.CurrentUserId <= 0) return Fail(MessageKeys.CurrentUserIdInvalid);

            FriendRequestDto friendRequest = friendRepository.GetFriendRequestById(requestDto.FriendRequestId);

            if (friendRequest == null) return Fail(MessageKeys.FriendRequestNotFound);
            if (friendRequest.Status != PendingStatus) return Fail(MessageKeys.FriendRequestNotPending);
            if (mustBeReceiver && friendRequest.ReceiverUserId != requestDto.CurrentUserId) return Fail(MessageKeys.OnlyReceiverCanRespond);
            if (!mustBeReceiver && friendRequest.SenderUserId != requestDto.CurrentUserId) return Fail(MessageKeys.OnlySenderCanCancel);

            return Success(MessageKeys.ValidationSuccessful, friendRequest: friendRequest);
        }

        private FriendOperationResultDto Success(
            string messageKey,
            FriendRequestDto friendRequest = null,
            System.Collections.Generic.List<FriendRequestDto> friendRequests = null,
            System.Collections.Generic.List<FriendDto> friends = null)
        {
            return new FriendOperationResultDto
            {
                Success = true,
                MessageKey = messageKey,
                Message = MessageLocalizer.Get(messageKey),
                FriendRequest = friendRequest,
                FriendRequests = friendRequests,
                Friends = friends
            };
        }

        private FriendOperationResultDto Fail(string messageKey)
        {
            return new FriendOperationResultDto
            {
                Success = false,
                MessageKey = messageKey,
                Message = MessageLocalizer.Get(messageKey)
            };
        }
    }
}
