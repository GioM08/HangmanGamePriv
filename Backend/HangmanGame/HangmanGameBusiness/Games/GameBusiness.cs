using System;
using HangmanGameBusiness.Localization;
using HangmanGameData.Repositories;
using HangmanGameEntities.Dtos;

namespace HangmanGameBusiness.Games
{
    public class GameBusiness : IGameBusiness
    {
        private readonly IGameRepository _gameRepository;

        public GameBusiness()
        {
            _gameRepository = new GameRepository();
        }

        public GameBusiness(IGameRepository gameRepository)
        {
            _gameRepository = gameRepository;
        }

        public GameOperationResultDto CreateGame(CreateGameDto createGameDto)
        {
            try
            {
                if (createGameDto == null)
                    return Fail(MessageKeys.GameDataRequired);
                if (createGameDto.CreatorId <= 0)
                    return Fail(MessageKeys.CreatorIdInvalid);
                if (createGameDto.WordId <= 0)
                    return Fail(MessageKeys.WordRequired);

                var game = _gameRepository.CreateGame(createGameDto);
                return Success(MessageKeys.GameCreatedSuccessfully, game: game);
            }
            catch (Exception)
            {
                return Fail(MessageKeys.UnexpectedError);
            }
        }

        public GameOperationResultDto JoinGame(int gameId, int retadorId)
        {
            try
            {
                if (gameId <= 0) return Fail(MessageKeys.GameIdInvalid);
                if (retadorId <= 0) return Fail(MessageKeys.PlayerIdInvalid);

                var game = _gameRepository.JoinGame(gameId, retadorId);
                if (game == null) return Fail(MessageKeys.GameNotAvailable);

                return Success(MessageKeys.JoinedGameSuccessfully, game: game);
            }
            catch (Exception)
            {
                return Fail(MessageKeys.UnexpectedError);
            }
        }

        public GameOperationResultDto GetAvailableGames()
        {
            try
            {
                var games = _gameRepository.GetAvailableGames();
                return Success(MessageKeys.GamesRetrievedSuccessfully, games: games);
            }
            catch (Exception)
            {
                return Fail(MessageKeys.UnexpectedError);
            }
        }

        public GameOperationResultDto GetGameById(int gameId)
        {
            try
            {
                if (gameId <= 0) return Fail(MessageKeys.GameIdInvalid);
                var game = _gameRepository.GetGameById(gameId);
                if (game == null) return Fail(MessageKeys.GameNotFound);
                return Success(MessageKeys.GameRetrievedSuccessfully, game: game);
            }
            catch (Exception)
            {
                return Fail(MessageKeys.UnexpectedError);
            }
        }

        public GameOperationResultDto GetGameState(int gameId)
        {
            try
            {
                if (gameId <= 0) return Fail(MessageKeys.GameIdInvalid);
                var state = _gameRepository.GetGameState(gameId);
                if (state == null) return Fail(MessageKeys.GameNotFound);
                return Success(MessageKeys.GameStateRetrievedSuccessfully, gameState: state);
            }
            catch (Exception)
            {
                return Fail(MessageKeys.UnexpectedError);
            }
        }

        public GameOperationResultDto GuessLetter(GuessLetterDto guessLetterDto)
        {
            try
            {
                if (guessLetterDto == null) return Fail(MessageKeys.GuessDataRequired);
                if (guessLetterDto.GameId <= 0) return Fail(MessageKeys.GameIdInvalid);
                if (guessLetterDto.UserId <= 0) return Fail(MessageKeys.PlayerIdInvalid);
                if (string.IsNullOrWhiteSpace(guessLetterDto.Letter)) return Fail(MessageKeys.LetterRequired);

                var state = _gameRepository.GuessLetter(guessLetterDto);
                if (state == null) return Fail(MessageKeys.LetterProcessingFailed);
                return Success(MessageKeys.LetterProcessedSuccessfully, gameState: state);
            }
            catch (Exception)
            {
                return Fail(MessageKeys.UnexpectedError);
            }
        }

        public GameOperationResultDto AbandonGame(int gameId, int userId)
        {
            try
            {
                if (gameId <= 0 || userId <= 0) return Fail(MessageKeys.InvalidData);
                bool ok = _gameRepository.AbandonGame(gameId, userId);
                return ok
                    ? Success(MessageKeys.GameAbandonedSuccessfully)
                    : Fail(MessageKeys.GameAbandonFailed);
            }
            catch (Exception)
            {
                return Fail(MessageKeys.UnexpectedError);
            }
        }

        public GameOperationResultDto GetUserScore(int userId)
        {
            try
            {
                if (userId <= 0) return Fail(MessageKeys.UserIdInvalid);
                var score = _gameRepository.GetUserScore(userId);
                if (score == null) return Fail(MessageKeys.UserNotFound);
                return Success(MessageKeys.UserScoreRetrievedSuccessfully, userScore: score);
            }
            catch (Exception)
            {
                return Fail(MessageKeys.UnexpectedError);
            }
        }

        private static GameOperationResultDto Success(
            string messageKey,
            GameDto game = null,
            GameStateDto gameState = null,
            System.Collections.Generic.List<GameDto> games = null,
            UserScoreDto userScore = null)
            => new GameOperationResultDto
            {
                Success = true,
                MessageKey = messageKey,
                Message = MessageLocalizer.Get(messageKey),
                Game = game,
                GameState = gameState,
                Games = games,
                UserScore = userScore
            };

        private static GameOperationResultDto Fail(string messageKey)
            => new GameOperationResultDto
            {
                Success = false,
                MessageKey = messageKey,
                Message = MessageLocalizer.Get(messageKey)
            };
    }
}
