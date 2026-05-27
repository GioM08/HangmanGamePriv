using System;
using System.Collections.Generic;
using System.Linq;
using HangmanGameData.Repositories.Interfaces;
using HangmanGameEntities.Dtos;

namespace HangmanGameData.Repositories
{
    public class FriendRepository : IFriendRepository
    {
        private const int PendingStatus = 0;
        private const int AcceptedStatus = 1;
        private const int RejectedStatus = 2;
        private const int CancelledStatus = 3;
        private const int RemovedStatus = 4;

        public bool UserExists(int userId)
        {
            using (var database = new HangmanGameDataContext())
            {
                return database.Users.Any(user => user.UserId == userId && user.IsActive == true);
            }
        }

        public bool ActiveFriendRelationExists(int firstUserId, int secondUserId)
        {
            int userAId = Math.Min(firstUserId, secondUserId);
            int userBId = Math.Max(firstUserId, secondUserId);

            using (var database = new HangmanGameDataContext())
            {
                return database.FriendRequests.Any(request =>
                    request.UserAId == userAId &&
                    request.UserBId == userBId &&
                    (request.Status == PendingStatus || request.Status == AcceptedStatus));
            }
        }

        public FriendRequestDto SendFriendRequest(SendFriendRequestDto requestDto)
        {
            using (var database = new HangmanGameDataContext())
            {
                FriendRequests friendRequest = new FriendRequests();

                friendRequest.SenderUserId = requestDto.SenderUserId;
                friendRequest.ReceiverUserId = requestDto.ReceiverUserId;
                friendRequest.Status = PendingStatus;
                friendRequest.CreatedAt = DateTime.Now;

                database.FriendRequests.InsertOnSubmit(friendRequest);
                database.SubmitChanges();

                return MapToFriendRequestDto(database, friendRequest);
            }
        }

        public List<FriendRequestDto> GetPendingFriendRequests(int userId)
        {
            using (var database = new HangmanGameDataContext())
            {
                List<FriendRequests> requests = database.FriendRequests
                    .Where(request =>
                        request.ReceiverUserId == userId &&
                        request.Status == PendingStatus)
                    .OrderByDescending(request => request.CreatedAt)
                    .ToList();

                return requests
                    .Select(request => MapToFriendRequestDto(database, request))
                    .ToList();
            }
        }

        public List<FriendRequestDto> GetSentFriendRequests(int userId)
        {
            using (var database = new HangmanGameDataContext())
            {
                List<FriendRequests> requests = database.FriendRequests
                    .Where(request =>
                        request.SenderUserId == userId &&
                        request.Status == PendingStatus)
                    .OrderByDescending(request => request.CreatedAt)
                    .ToList();

                return requests
                    .Select(request => MapToFriendRequestDto(database, request))
                    .ToList();
            }
        }

        public FriendRequestDto GetFriendRequestById(int friendRequestId)
        {
            using (var database = new HangmanGameDataContext())
            {
                FriendRequests request = database.FriendRequests
                    .FirstOrDefault(item => item.FriendRequestId == friendRequestId);

                if (request == null)
                {
                    return null;
                }

                return MapToFriendRequestDto(database, request);
            }
        }

        public FriendRequestDto GetAcceptedFriendRelation(int firstUserId, int secondUserId)
        {
            int userAId = Math.Min(firstUserId, secondUserId);
            int userBId = Math.Max(firstUserId, secondUserId);

            using (var database = new HangmanGameDataContext())
            {
                FriendRequests request = database.FriendRequests
                    .FirstOrDefault(item =>
                        item.UserAId == userAId &&
                        item.UserBId == userBId &&
                        item.Status == AcceptedStatus);

                if (request == null)
                {
                    return null;
                }

                return MapToFriendRequestDto(database, request);
            }
        }

        public FriendRequestDto AcceptFriendRequest(int friendRequestId)
        {
            using (var database = new HangmanGameDataContext())
            {
                FriendRequests request = database.FriendRequests
                    .FirstOrDefault(item => item.FriendRequestId == friendRequestId);

                if (request == null)
                {
                    return null;
                }

                request.Status = AcceptedStatus;
                request.UpdatedAt = DateTime.Now;

                database.SubmitChanges();

                return MapToFriendRequestDto(database, request);
            }
        }

        public FriendRequestDto RejectFriendRequest(int friendRequestId)
        {
            using (var database = new HangmanGameDataContext())
            {
                FriendRequests request = database.FriendRequests
                    .FirstOrDefault(item => item.FriendRequestId == friendRequestId);

                if (request == null)
                {
                    return null;
                }

                request.Status = RejectedStatus;
                request.UpdatedAt = DateTime.Now;

                database.SubmitChanges();

                return MapToFriendRequestDto(database, request);
            }
        }

        public FriendRequestDto CancelFriendRequest(int friendRequestId)
        {
            using (var database = new HangmanGameDataContext())
            {
                FriendRequests request = database.FriendRequests
                    .FirstOrDefault(item => item.FriendRequestId == friendRequestId);

                if (request == null)
                {
                    return null;
                }

                request.Status = CancelledStatus;
                request.UpdatedAt = DateTime.Now;

                database.SubmitChanges();

                return MapToFriendRequestDto(database, request);
            }
        }

        public FriendRequestDto RemoveFriend(int friendRequestId)
        {
            using (var database = new HangmanGameDataContext())
            {
                FriendRequests request = database.FriendRequests
                    .FirstOrDefault(item => item.FriendRequestId == friendRequestId);

                if (request == null)
                {
                    return null;
                }

                request.Status = RemovedStatus;
                request.UpdatedAt = DateTime.Now;

                database.SubmitChanges();

                return MapToFriendRequestDto(database, request);
            }
        }

        public List<FriendDto> GetFriends(int userId)
        {
            using (var database = new HangmanGameDataContext())
            {
                List<FriendRequests> acceptedRequests = database.FriendRequests
                    .Where(request =>
                        request.Status == AcceptedStatus &&
                        (request.SenderUserId == userId || request.ReceiverUserId == userId))
                    .OrderByDescending(request => request.UpdatedAt ?? request.CreatedAt)
                    .ToList();

                List<FriendDto> friends = new List<FriendDto>();

                foreach (FriendRequests request in acceptedRequests)
                {
                    int friendUserId = request.SenderUserId == userId
                        ? request.ReceiverUserId
                        : request.SenderUserId;

                    Users friendUser = database.Users
                        .FirstOrDefault(user => user.UserId == friendUserId && user.IsActive == true);

                    if (friendUser != null)
                    {
                        friends.Add(new FriendDto
                        {
                            UserId = friendUser.UserId,
                            FullName = friendUser.FullName,
                            Email = friendUser.Email,
                            GlobalScore = friendUser.GlobalScore,
                            FriendsSince = request.UpdatedAt ?? request.CreatedAt
                        });
                    }
                }

                return friends;
            }
        }

        private FriendRequestDto MapToFriendRequestDto(
            HangmanGameDataContext database,
            FriendRequests request)
        {
            Users sender = database.Users
                .FirstOrDefault(user => user.UserId == request.SenderUserId);

            Users receiver = database.Users
                .FirstOrDefault(user => user.UserId == request.ReceiverUserId);

            return new FriendRequestDto
            {
                FriendRequestId = request.FriendRequestId,

                SenderUserId = request.SenderUserId,
                SenderFullName = sender != null ? sender.FullName : string.Empty,
                SenderEmail = sender != null ? sender.Email : string.Empty,

                ReceiverUserId = request.ReceiverUserId,
                ReceiverFullName = receiver != null ? receiver.FullName : string.Empty,
                ReceiverEmail = receiver != null ? receiver.Email : string.Empty,

                Status = request.Status,
                CreatedAt = request.CreatedAt
            };
        }

        public int GetUserIdByEmail(string email)
        {
            using (var database = new HangmanGameDataContext())
            {
                string normalizedEmail = email.Trim().ToLower();

                Users userFound = database.Users
                    .FirstOrDefault(user =>
                        user.Email.ToLower() == normalizedEmail &&
                        user.IsActive == true);

                if (userFound == null)
                {
                    return 0;
                }

                return userFound.UserId;
            }
        }


    }
}
