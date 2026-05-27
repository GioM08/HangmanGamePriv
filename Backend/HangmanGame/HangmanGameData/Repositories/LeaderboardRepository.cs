using System.Collections.Generic;
using System.Linq;
using HangmanGameData.Repositories.Interfaces;
using HangmanGameEntities.Dtos;

namespace HangmanGameData.Repositories
{
    public class LeaderboardRepository : ILeaderboardRepository
    {
        public bool UserExists(int userId)
        {
            using (var database = new HangmanGameDataContext())
            {
                return database.Users.Any(user =>
                    user.UserId == userId &&
                    user.IsActive == true);
            }
        }

        public List<LeaderboardPlayerDto> GetTopScoreLeaderboard()
        {
            using (var database = new HangmanGameDataContext())
            {
                List<LeaderboardPlayerDto> ranking = BuildScoreRanking(database);

                return ranking
                    .Take(10)
                    .ToList();
            }
        }

        public LeaderboardPlayerDto GetPlayerScoreRank(int userId)
        {
            using (var database = new HangmanGameDataContext())
            {
                List<LeaderboardPlayerDto> ranking = BuildScoreRanking(database);

                return ranking.FirstOrDefault(player => player.UserId == userId);
            }
        }

        private List<LeaderboardPlayerDto> BuildScoreRanking(HangmanGameDataContext database)
        {
            var users = database.Users
                .Where(user => user.IsActive == true)
                .OrderByDescending(user => user.GlobalScore)
                .ThenBy(user => user.FullName)
                .ThenBy(user => user.UserId)
                .ToList();

            List<LeaderboardPlayerDto> ranking = new List<LeaderboardPlayerDto>();

            for (int index = 0; index < users.Count; index++)
            {
                ranking.Add(new LeaderboardPlayerDto
                {
                    Position = index + 1,
                    UserId = users[index].UserId,
                    FullName = users[index].FullName,
                    GlobalScore = users[index].GlobalScore
                });
            }

            return ranking;
        }
    }
}
