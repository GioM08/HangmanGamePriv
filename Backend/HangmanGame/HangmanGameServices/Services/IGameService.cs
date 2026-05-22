using System.ServiceModel;
using HangmanGameEntities.Dtos;

namespace HangmanGameServices.Services
{
    [ServiceContract(CallbackContract = typeof(IGameCallback), SessionMode = SessionMode.Required)]
    public interface IGameService
    {
        [OperationContract]
        GameOperationResultDto CreateGame(CreateGameDto createGameDto);

        [OperationContract]
        GameOperationResultDto JoinGame(int gameId, int retadorId);

        [OperationContract]
        GameOperationResultDto GetAvailableGames();

        [OperationContract]
        GameOperationResultDto GetGameState(int gameId);

        [OperationContract]
        GameOperationResultDto GuessLetter(GuessLetterDto guessLetterDto);

        [OperationContract]
        GameOperationResultDto AbandonGame(int gameId, int userId);

        [OperationContract]
        GameOperationResultDto GetUserScore(int userId);

        [OperationContract(IsOneWay = true)]
        void RegisterForGame(int gameId, int userId);
    }
}
