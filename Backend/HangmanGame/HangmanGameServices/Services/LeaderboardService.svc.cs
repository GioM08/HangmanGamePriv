using HangmanGameBusiness.Leaderboards;
using HangmanGameEntities.Dtos;

namespace HangmanGameServices.Services
{
    public class LeaderboardService : ILeaderboardService
    {
        private readonly ILeaderboardBusiness leaderboardBusiness;

        public LeaderboardService()
        {
            this.leaderboardBusiness = new LeaderboardBusiness();
        }

        public LeaderboardOperationResultDto GetTopScoreLeaderboard(int currentUserId)
        {
            return leaderboardBusiness.GetTopScoreLeaderboard(currentUserId);
        }
    }
}
