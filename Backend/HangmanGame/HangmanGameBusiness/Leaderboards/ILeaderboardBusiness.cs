using HangmanGameEntities.Dtos;

namespace HangmanGameBusiness.Leaderboards
{
    public interface ILeaderboardBusiness
    {
        LeaderboardOperationResultDto GetTopScoreLeaderboard(int currentUserId);
    }
}
