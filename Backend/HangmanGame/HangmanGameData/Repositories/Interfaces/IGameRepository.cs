using System.Collections.Generic;
using HangmanGameEntities.Dtos;

namespace HangmanGameData.Repositories
{
    public interface IGameRepository
    {
        GameDto CreateGame(CreateGameDto createGameDto);
        GameDto JoinGame(int gameId, int retadorId);
        GameDto GetGameById(int gameId);
        List<GameDto> GetAvailableGames();
        GameStateDto GetGameState(int gameId);
        GameStateDto GuessLetter(GuessLetterDto guessLetterDto);
        bool AbandonGame(int gameId, int userId);
        UserScoreDto GetUserScore(int userId);
    }
}
