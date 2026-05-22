using System;
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
                    return Fail("Datos de partida requeridos.");
                if (createGameDto.CreatorId <= 0)
                    return Fail("ID de creador invalido.");
                if (createGameDto.WordId <= 0)
                    return Fail("Debe seleccionar una palabra.");

                var game = _gameRepository.CreateGame(createGameDto);
                return new GameOperationResultDto { Success = true, Message = "Partida creada.", Game = game };
            }
            catch (Exception)
            {
                return Fail("Error al crear la partida.");
            }
        }

        public GameOperationResultDto JoinGame(int gameId, int retadorId)
        {
            try
            {
                if (gameId <= 0) return Fail("ID de partida invalido.");
                if (retadorId <= 0) return Fail("ID de jugador invalido.");

                var game = _gameRepository.JoinGame(gameId, retadorId);
                if (game == null) return Fail("Partida no disponible.");

                return new GameOperationResultDto { Success = true, Message = "Te uniste a la partida.", Game = game };
            }
            catch (Exception)
            {
                return Fail("Error al unirse a la partida.");
            }
        }

        public GameOperationResultDto GetAvailableGames()
        {
            try
            {
                var games = _gameRepository.GetAvailableGames();
                return new GameOperationResultDto { Success = true, Games = games };
            }
            catch (Exception)
            {
                return Fail("Error al obtener partidas.");
            }
        }

        public GameOperationResultDto GetGameById(int gameId)
        {
            try
            {
                if (gameId <= 0) return Fail("ID de partida invalido.");
                var game = _gameRepository.GetGameById(gameId);
                if (game == null) return Fail("Partida no encontrada.");
                return new GameOperationResultDto { Success = true, Game = game };
            }
            catch (Exception)
            {
                return Fail("Error al obtener la partida.");
            }
        }

        public GameOperationResultDto GetGameState(int gameId)
        {
            try
            {
                if (gameId <= 0) return Fail("ID de partida invalido.");
                var state = _gameRepository.GetGameState(gameId);
                if (state == null) return Fail("Partida no encontrada.");
                return new GameOperationResultDto { Success = true, GameState = state };
            }
            catch (Exception)
            {
                return Fail("Error al obtener estado de la partida.");
            }
        }

        public GameOperationResultDto GuessLetter(GuessLetterDto guessLetterDto)
        {
            try
            {
                if (guessLetterDto == null) return Fail("Datos requeridos.");
                if (guessLetterDto.GameId <= 0) return Fail("ID de partida invalido.");
                if (guessLetterDto.UserId <= 0) return Fail("ID de jugador invalido.");
                if (string.IsNullOrWhiteSpace(guessLetterDto.Letter)) return Fail("Letra requerida.");

                var state = _gameRepository.GuessLetter(guessLetterDto);
                if (state == null) return Fail("Error al procesar la letra.");
                return new GameOperationResultDto { Success = true, GameState = state };
            }
            catch (Exception)
            {
                return Fail("Error al procesar la jugada.");
            }
        }

        public GameOperationResultDto AbandonGame(int gameId, int userId)
        {
            try
            {
                if (gameId <= 0 || userId <= 0) return Fail("Datos invalidos.");
                bool ok = _gameRepository.AbandonGame(gameId, userId);
                return ok
                    ? new GameOperationResultDto { Success = true, Message = "Partida abandonada. Se aplico penalizacion." }
                    : Fail("No se pudo abandonar la partida.");
            }
            catch (Exception)
            {
                return Fail("Error al abandonar la partida.");
            }
        }

        public GameOperationResultDto GetUserScore(int userId)
        {
            try
            {
                if (userId <= 0) return Fail("ID de usuario invalido.");
                var score = _gameRepository.GetUserScore(userId);
                if (score == null) return Fail("Usuario no encontrado.");
                return new GameOperationResultDto { Success = true, UserScore = score };
            }
            catch (Exception)
            {
                return Fail("Error al obtener puntaje.");
            }
        }

        private static GameOperationResultDto Fail(string message)
            => new GameOperationResultDto { Success = false, Message = message };
    }
}
