using System.ServiceModel;
using HangmanGameEntities.Dtos;

namespace HangmanGameServices.Services
{
    [ServiceContract]
    public interface ILeaderboardService
    {
        [OperationContract]
        LeaderboardOperationResultDto GetTopScoreLeaderboard(int currentUserId);
    }
}
