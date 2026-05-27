using System.Collections.Generic;
using HangmanGameEntities.Dtos;

namespace HangmanGameData.Repositories.Interfaces
{
    public interface ILeaderboardRepository
    {
        bool UserExists(int userId);

        List<LeaderboardPlayerDto> GetTopScoreLeaderboard();

        LeaderboardPlayerDto GetPlayerScoreRank(int userId);
    }
}
