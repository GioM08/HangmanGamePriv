using HangmanGameBusiness.Leaderboards;
using HangmanGameBusiness.Localization;
using HangmanGameEntities.Dtos;
using HangmanGameServices.Localization;

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
            SetLanguage();
            return leaderboardBusiness.GetTopScoreLeaderboard(currentUserId);
        }

        private static void SetLanguage()
        {
            LanguageContext.SetLanguage(RequestLanguageReader.GetLanguageCode());
        }
    }
}
