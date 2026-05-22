using HangmanGameEntities.Dtos;

namespace HangmanGameBusiness.Games
{
    public interface IGameBusiness
    {
        GameOperationResultDto CreateGame(CreateGameDto createGameDto);
        GameOperationResultDto JoinGame(int gameId, int retadorId);
        GameOperationResultDto GetAvailableGames();
        GameOperationResultDto GetGameById(int gameId);
        GameOperationResultDto GetGameState(int gameId);
        GameOperationResultDto GuessLetter(GuessLetterDto guessLetterDto);
        GameOperationResultDto AbandonGame(int gameId, int userId);
        GameOperationResultDto GetUserScore(int userId);
    }
}
