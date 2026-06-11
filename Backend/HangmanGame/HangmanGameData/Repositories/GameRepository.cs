using System;
using System.Collections.Generic;
using System.Linq;
using HangmanGameEntities.Dtos;

namespace HangmanGameData.Repositories
{
    public class GameRepository : IGameRepository
    {
        private const int MaxIncorrectAttempts = 6;
        private const int PointsWinner = 10;
        private const int PointsPenalty = -3;

        public GameDto CreateGame(CreateGameDto createGameDto)
        {
            using (var db = new HangmanGameDataContext())
            {
                var game = new Games
                {
                    CreatorId = createGameDto.CreatorId,
                    WordId = createGameDto.WordId,
                    Status = 0,
                    Description = createGameDto.Description,
                    CreatedAt = DateTime.Now
                };

                db.Games.InsertOnSubmit(game);
                db.SubmitChanges();

                return BuildGameDto(db, game);
            }
        }

        public GameDto JoinGame(int gameId, int retadorId)
        {
            using (var db = new HangmanGameDataContext())
            {
                var game = db.Games.FirstOrDefault(g => g.GameId == gameId && g.Status == 0);
                if (game == null) return null;

                game.RetadorId = retadorId;
                game.Status = 1;
                game.StartedAt = DateTime.Now;
                db.SubmitChanges();

                return BuildGameDto(db, game);
            }
        }

        public GameDto GetGameById(int gameId)
        {
            using (var db = new HangmanGameDataContext())
            {
                var game = db.Games.FirstOrDefault(g => g.GameId == gameId);
                return game == null ? null : BuildGameDto(db, game);
            }
        }

        public List<GameDto> GetAvailableGames()
        {
            using (var db = new HangmanGameDataContext())
            {
                var games = db.Games.Where(g => g.Status == 0).ToList();
                return games.Select(g => BuildGameDto(db, g)).ToList();
            }
        }

        public GameStateDto GetGameState(int gameId)
        {
            using (var db = new HangmanGameDataContext())
            {
                var game = db.Games.FirstOrDefault(g => g.GameId == gameId);
                if (game == null) return null;
                return BuildGameState(db, game);
            }
        }

        public GameStateDto GuessLetter(GuessLetterDto guessLetterDto)
        {
            using (var db = new HangmanGameDataContext())
            {
                var game = db.Games.FirstOrDefault(g => g.GameId == guessLetterDto.GameId && g.Status == 1);
                if (game == null)
                    return new GameStateDto { Message = "Partida no encontrada o no activa.", IsOver = true };

                if (string.IsNullOrWhiteSpace(guessLetterDto.Letter))
                {
                    return new GameStateDto
                    {
                        Message = "Debe ingresar una letra.",
                        IsOver = false
                    };
                }

                string normalizedLetter = guessLetterDto.Letter.Trim().ToUpper();

                if (normalizedLetter.Length != 1)
                {
                    return new GameStateDto
                    {
                        Message = "Solo se permite ingresar una letra.",
                        IsOver = false
                    };
                }

                char letter = normalizedLetter[0];

                var alreadyUsed = db.GameMoves.Any(m =>
                    m.GameId == guessLetterDto.GameId &&
                    m.Letter == letter);

                if (alreadyUsed)
                {
                    return BuildGameState(db, game, letter.ToString(), null, "Letra ya utilizada.");
                }

                var word = db.Words.FirstOrDefault(w => w.WordId == game.WordId);
                if (word == null) return null;

                bool isCorrect = word.Text.ToUpper().Contains(letter.ToString());

                var move = new GameMoves
                {
                    GameId = guessLetterDto.GameId,
                    UserId = guessLetterDto.UserId,
                    Letter = letter,
                    IsCorrect = isCorrect,
                    MoveDate = DateTime.Now
                };
                db.GameMoves.InsertOnSubmit(move);
                db.SubmitChanges();

                var state = BuildGameState(db, game, letter.ToString(), isCorrect, null);

                if (state.IsOver)
                {
                    game.Status = 2;
                    game.FinishedAt = DateTime.Now;

                    if (state.WinnerId.HasValue)
                    {
                        game.WinnerId = state.WinnerId;
                        ApplyScoreChange(db, state.WinnerId.Value, PointsWinner);

                        int loserId = state.WinnerId.Value == game.CreatorId ? game.RetadorId.Value : game.CreatorId;
                        ApplyScoreChange(db, loserId, 0);
                    }
                    db.SubmitChanges();
                }

                return state;
            }
        }

        public bool AbandonGame(int gameId, int userId)
        {
            using (var db = new HangmanGameDataContext())
            {
                var game = db.Games.FirstOrDefault(g => g.GameId == gameId && g.Status == 1);
                if (game == null) return false;

                game.Status = 3;
                game.FinishedAt = DateTime.Now;
                game.AbandonedByUserId = userId;

                int winnerId = userId == game.CreatorId ? (game.RetadorId ?? 0) : game.CreatorId;
                if (winnerId > 0)
                {
                    game.WinnerId = winnerId;
                    ApplyScoreChange(db, winnerId, PointsWinner);
                }
                ApplyScoreChange(db, userId, PointsPenalty);

                db.SubmitChanges();
                return true;
            }
        }

        public UserScoreDto GetUserScore(int userId)
        {
            using (var db = new HangmanGameDataContext())
            {
                var user = db.Users.FirstOrDefault(u => u.UserId == userId);
                if (user == null) return null;

                var finishedGames = db.Games
                    .Where(g => g.Status == 2 || g.Status == 3)
                    .Where(g => g.CreatorId == userId || g.RetadorId == userId)
                    .ToList();

                var wonGames = new List<GameResultDto>();
                var lostGames = new List<GameResultDto>();
                var penalties = new List<PenaltyDto>();

                foreach (var game in finishedGames)
                {
                    var word = db.Words.FirstOrDefault(w => w.WordId == game.WordId);
                    string wordText = word?.Text ?? "???";

                    int opponentId = game.CreatorId == userId ? (game.RetadorId ?? 0) : game.CreatorId;
                    var opponent = opponentId > 0 ? db.Users.FirstOrDefault(u => u.UserId == opponentId) : null;
                    string opponentName = opponent?.FullName ?? "Desconocido";

                    if (game.Status == 3 && game.AbandonedByUserId == userId)
                    {
                        penalties.Add(new PenaltyDto
                        {
                            PenaltyDate = game.FinishedAt ?? game.CreatedAt,
                            Word = wordText,
                            Reason = "Abandono de partida",
                            PointsDeducted = Math.Abs(PointsPenalty)
                        });
                    }
                    else if (game.WinnerId == userId)
                    {
                        wonGames.Add(new GameResultDto
                        {
                            GameDate = game.FinishedAt ?? game.CreatedAt,
                            Word = wordText,
                            OpponentName = opponentName,
                            ResultType = "Ganado",
                            PointsEarned = PointsWinner
                        });
                    }
                    else
                    {
                        lostGames.Add(new GameResultDto
                        {
                            GameDate = game.FinishedAt ?? game.CreatedAt,
                            Word = wordText,
                            OpponentName = opponentName,
                            ResultType = "Perdido",
                            PointsEarned = 0
                        });
                    }
                }

                return new UserScoreDto
                {
                    TotalPoints = user.GlobalScore,
                    GamesPlayed = finishedGames.Count,
                    GamesWon = wonGames.Count,
                    GamesLost = lostGames.Count,
                    WonGames = wonGames,
                    LostGames = lostGames,
                    Penalties = penalties
                };
            }
        }

        private GameDto BuildGameDto(HangmanGameDataContext db, Games game)
        {
            var creator = db.Users.FirstOrDefault(u => u.UserId == game.CreatorId);
            var retador = game.RetadorId.HasValue ? db.Users.FirstOrDefault(u => u.UserId == game.RetadorId.Value) : null;
            var word = db.Words.FirstOrDefault(w => w.WordId == game.WordId);
            var category = word != null ? db.Categories.FirstOrDefault(c => c.CategoryId == word.CategoryId) : null;

            return new GameDto
            {
                GameId = game.GameId,
                CreatorId = game.CreatorId,
                CreatorName = creator?.FullName ?? string.Empty,
                CreatorEmail = creator?.Email ?? string.Empty,
                RetadorId = game.RetadorId,
                RetadorName = retador?.FullName,
                WordLength = word?.Text?.Length ?? 0,
                Category = category?.Name ?? string.Empty,
                LanguageCode = word?.LanguageCode ?? string.Empty,
                Description = game.Description,
                Status = game.Status,
                CreatedAt = game.CreatedAt
            };
        }

        private GameStateDto BuildGameState(HangmanGameDataContext db, Games game,
            string lastLetter = null, bool? lastCorrect = null, string message = null)
        {
            var word = db.Words.FirstOrDefault(w => w.WordId == game.WordId);
            string wordText = word?.Text?.ToUpper() ?? string.Empty;

            var moves = db.GameMoves.Where(m => m.GameId == game.GameId).ToList();
            var usedLetters = moves
    .Select(m => char.ToUpper(m.Letter).ToString())
    .Distinct()
    .ToList();

            int incorrectCount = moves.Count(m => !m.IsCorrect);

            string revealed = string.Join(" ", wordText.Select(c =>
                char.IsLetter(c) && usedLetters.Contains(c.ToString()) ? c.ToString() : "_"));

            bool wordGuessed = !revealed.Contains("_");
            bool hangmanComplete = incorrectCount >= MaxIncorrectAttempts;
            bool abandoned = game.Status == 3;
            bool isOver = wordGuessed || hangmanComplete || abandoned;

            int? winnerId = null;
            if (abandoned) winnerId = game.WinnerId;
            else if (wordGuessed) winnerId = game.RetadorId;
            else if (hangmanComplete) winnerId = game.CreatorId;

            return new GameStateDto
            {
                GameId = game.GameId,
                RevealedWord = revealed,
                IncorrectAttempts = incorrectCount,
                UsedLetters = usedLetters,
                IsOver = isOver,
                WinnerId = winnerId,
                Status = game.Status,
                LastLetter = lastLetter,
                LastGuessCorrect = lastCorrect ?? false,
                Message = message ?? (abandoned
                    ? "El otro jugador abandono la partida."
                    : isOver
                        ? (wordGuessed ? "Palabra adivinada!" : "Ahorcado completo. Perdiste.")
                        : (lastLetter != null
                            ? (lastCorrect == true ? $"¡La letra '{lastLetter}' esta en la palabra!" : $"La letra '{lastLetter}' no esta en la palabra.")
                            : string.Empty))
            };
        }

        private static void ApplyScoreChange(HangmanGameDataContext db, int userId, int delta)
        {
            var user = db.Users.FirstOrDefault(u => u.UserId == userId);
            if (user == null) return;
            user.GlobalScore += delta;
        }
    }
}
