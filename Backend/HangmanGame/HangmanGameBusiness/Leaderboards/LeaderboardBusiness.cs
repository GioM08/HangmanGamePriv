using System;
using HangmanGameBusiness.Localization;
using HangmanGameData.Repositories;
using HangmanGameData.Repositories.Interfaces;
using HangmanGameEntities.Dtos;

namespace HangmanGameBusiness.Leaderboards
{
    public class LeaderboardBusiness : ILeaderboardBusiness
    {
        private readonly ILeaderboardRepository leaderboardRepository;

        public LeaderboardBusiness()
        {
            this.leaderboardRepository = new LeaderboardRepository();
        }

        public LeaderboardBusiness(ILeaderboardRepository leaderboardRepository)
        {
            if (leaderboardRepository == null)
            {
                throw new ArgumentNullException(nameof(leaderboardRepository));
            }

            this.leaderboardRepository = leaderboardRepository;
        }

        public LeaderboardOperationResultDto GetTopScoreLeaderboard(int currentUserId)
        {
            try
            {
                if (currentUserId <= 0)
                {
                    return Fail(MessageKeys.CurrentUserIdInvalid);
                }

                if (!leaderboardRepository.UserExists(currentUserId))
                {
                    return Fail(MessageKeys.CurrentUserNotFound);
                }

                return new LeaderboardOperationResultDto
                {
                    Success = true,
                    MessageKey = MessageKeys.LeaderboardRetrievedSuccessfully,
                    Message = MessageLocalizer.Get(MessageKeys.LeaderboardRetrievedSuccessfully),
                    TopPlayers = leaderboardRepository.GetTopScoreLeaderboard(),
                    CurrentPlayerRank = leaderboardRepository.GetPlayerScoreRank(currentUserId)
                };
            }
            catch (Exception)
            {
                return Fail(MessageKeys.UnexpectedError);
            }
        }

        private LeaderboardOperationResultDto Fail(string messageKey)
        {
            return new LeaderboardOperationResultDto
            {
                Success = false,
                MessageKey = messageKey,
                Message = MessageLocalizer.Get(messageKey)
            };
        }
    }
}
