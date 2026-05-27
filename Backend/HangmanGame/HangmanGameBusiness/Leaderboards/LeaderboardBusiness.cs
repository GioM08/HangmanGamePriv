using System;
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
                    return Fail("Current user id is not valid.");
                }

                if (!leaderboardRepository.UserExists(currentUserId))
                {
                    return Fail("Current user was not found.");
                }

                return new LeaderboardOperationResultDto
                {
                    Success = true,
                    Message = "Leaderboard retrieved successfully.",
                    TopPlayers = leaderboardRepository.GetTopScoreLeaderboard(),
                    CurrentPlayerRank = leaderboardRepository.GetPlayerScoreRank(currentUserId)
                };
            }
            catch (Exception)
            {
                return Fail("An unexpected error occurred while getting the leaderboard.");
            }
        }

        private LeaderboardOperationResultDto Fail(string message)
        {
            return new LeaderboardOperationResultDto
            {
                Success = false,
                Message = message
            };
        }
    }
}
