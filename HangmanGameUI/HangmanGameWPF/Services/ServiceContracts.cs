// Client-side WCF contract mirrors — must match server contracts exactly.
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;
using System;

namespace HangmanGameWPF.Services
{
    // ── DTOs — namespace debe coincidir exactamente con el servidor ───────────
    // Servidor: HangmanGameEntities.Dtos → XML ns = http://schemas.datacontract.org/2004/07/HangmanGameEntities.Dtos

    [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/HangmanGameEntities.Dtos")]
    public class UserDto
    {
        [DataMember] public int UserId { get; set; }
        [DataMember] public string FullName { get; set; }
        [DataMember] public DateTime BirthDate { get; set; }
        [DataMember] public string PhoneNumber { get; set; }
        [DataMember] public string Email { get; set; }
        [DataMember] public int GlobalScore { get; set; }
        [DataMember] public DateTime CreatedAt { get; set; }
    }

    [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/HangmanGameEntities.Dtos")] public class OperationResultDto
    {
        [DataMember] public bool Success { get; set; }
        [DataMember] public string Message { get; set; }
        [DataMember] public UserDto User { get; set; }
    }

    [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/HangmanGameEntities.Dtos")] public class LoginDto
    {
        [DataMember] public string Email { get; set; }
        [DataMember] public string Password { get; set; }
    }

    [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/HangmanGameEntities.Dtos")] public class RegisterUserDto
    {
        [DataMember] public string FullName { get; set; }
        [DataMember] public DateTime BirthDate { get; set; }
        [DataMember] public string PhoneNumber { get; set; }
        [DataMember] public string Email { get; set; }
        [DataMember] public string Password { get; set; }
    }

    [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/HangmanGameEntities.Dtos")] public class UpdateUserProfileDto
    {
        [DataMember] public int UserId { get; set; }
        [DataMember] public string FullName { get; set; }
        [DataMember] public DateTime BirthDate { get; set; }
        [DataMember] public string PhoneNumber { get; set; }
    }

    [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/HangmanGameEntities.Dtos")] public class GameDto
    {
        [DataMember] public int GameId { get; set; }
        [DataMember] public int CreatorId { get; set; }
        [DataMember] public string CreatorName { get; set; }
        [DataMember] public string CreatorEmail { get; set; }
        [DataMember] public int? RetadorId { get; set; }
        [DataMember] public string RetadorName { get; set; }
        [DataMember] public int WordLength { get; set; }
        [DataMember] public string Category { get; set; }
        [DataMember] public string Description { get; set; }
        [DataMember] public int Status { get; set; }
        [DataMember] public DateTime CreatedAt { get; set; }
    }

    [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/HangmanGameEntities.Dtos")] public class GameStateDto
    {
        [DataMember] public int GameId { get; set; }
        [DataMember] public string RevealedWord { get; set; }
        [DataMember] public int IncorrectAttempts { get; set; }
        [DataMember] public List<string> UsedLetters { get; set; }
        [DataMember] public bool IsOver { get; set; }
        [DataMember] public int? WinnerId { get; set; }
        [DataMember] public string Message { get; set; }
        [DataMember] public int Status { get; set; }
        [DataMember] public bool LastGuessCorrect { get; set; }
        [DataMember] public string LastLetter { get; set; }
    }

    [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/HangmanGameEntities.Dtos")] public class CategoryDto
    {
        [DataMember] public int CategoryId { get; set; }
        [DataMember] public string Name { get; set; }
        [DataMember] public string LanguageCode { get; set; }
    }

    [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/HangmanGameEntities.Dtos")] public class WordDto
    {
        [DataMember] public int WordId { get; set; }
        [DataMember] public string Text { get; set; }
        [DataMember] public string Hint { get; set; }
        [DataMember] public int CategoryId { get; set; }
        [DataMember] public string CategoryName { get; set; }
        [DataMember] public int Difficulty { get; set; }
    }

    [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/HangmanGameEntities.Dtos")] public class CreateGameDto
    {
        [DataMember] public int CreatorId { get; set; }
        [DataMember] public int WordId { get; set; }
        [DataMember] public string Description { get; set; }
    }

    [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/HangmanGameEntities.Dtos")] public class GuessLetterDto
    {
        [DataMember] public int GameId { get; set; }
        [DataMember] public int UserId { get; set; }
        [DataMember] public string Letter { get; set; }
    }

    [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/HangmanGameEntities.Dtos")] public class GameOperationResultDto
    {
        [DataMember] public bool Success { get; set; }
        [DataMember] public string Message { get; set; }
        [DataMember] public GameDto Game { get; set; }
        [DataMember] public GameStateDto GameState { get; set; }
        [DataMember] public List<GameDto> Games { get; set; }
        [DataMember] public List<WordDto> Words { get; set; }
        [DataMember] public List<CategoryDto> Categories { get; set; }
        [DataMember] public UserScoreDto UserScore { get; set; }
    }

    [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/HangmanGameEntities.Dtos")] public class UserScoreDto
    {
        [DataMember] public int TotalPoints { get; set; }
        [DataMember] public int GamesPlayed { get; set; }
        [DataMember] public int GamesWon { get; set; }
        [DataMember] public int GamesLost { get; set; }
        [DataMember] public List<GameResultDto> WonGames { get; set; }
        [DataMember] public List<GameResultDto> LostGames { get; set; }
        [DataMember] public List<PenaltyDto> Penalties { get; set; }
    }

    [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/HangmanGameEntities.Dtos")] public class GameResultDto
    {
        [DataMember] public DateTime GameDate { get; set; }
        [DataMember] public string Word { get; set; }
        [DataMember] public string OpponentName { get; set; }
        [DataMember] public string ResultType { get; set; }
        [DataMember] public int PointsEarned { get; set; }
    }

    [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/HangmanGameEntities.Dtos")] public class PenaltyDto
    {
        [DataMember] public DateTime PenaltyDate { get; set; }
        [DataMember] public string Word { get; set; }
        [DataMember] public string Reason { get; set; }
        [DataMember] public int PointsDeducted { get; set; }
    }

    [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/HangmanGameEntities.Dtos")]
    public class SendFriendRequestDto
    {
        [DataMember] public int SenderUserId { get; set; }
        [DataMember] public int ReceiverUserId { get; set; }
    }

    [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/HangmanGameEntities.Dtos")]
    public class RespondFriendRequestDto
    {
        [DataMember] public int FriendRequestId { get; set; }
        [DataMember] public int CurrentUserId { get; set; }
    }

    [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/HangmanGameEntities.Dtos")]
    public class RemoveFriendDto
    {
        [DataMember] public int CurrentUserId { get; set; }
        [DataMember] public int FriendUserId { get; set; }
    }

    [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/HangmanGameEntities.Dtos")]
    public class FriendRequestDto
    {
        [DataMember] public int FriendRequestId { get; set; }

        [DataMember] public int SenderUserId { get; set; }
        [DataMember] public string SenderFullName { get; set; }
        [DataMember] public string SenderEmail { get; set; }

        [DataMember] public int ReceiverUserId { get; set; }
        [DataMember] public string ReceiverFullName { get; set; }
        [DataMember] public string ReceiverEmail { get; set; }

        [DataMember] public int Status { get; set; }
        [DataMember] public DateTime CreatedAt { get; set; }
    }

    [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/HangmanGameEntities.Dtos")]
    public class FriendDto
    {
        [DataMember] public int UserId { get; set; }
        [DataMember] public string FullName { get; set; }
        [DataMember] public string Email { get; set; }
        [DataMember] public int GlobalScore { get; set; }
        [DataMember] public DateTime FriendsSince { get; set; }
    }

    [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/HangmanGameEntities.Dtos")]
    public class FriendOperationResultDto
    {
        [DataMember] public bool Success { get; set; }
        [DataMember] public string Message { get; set; }

        [DataMember] public FriendRequestDto FriendRequest { get; set; }
        [DataMember] public List<FriendRequestDto> FriendRequests { get; set; }
        [DataMember] public List<FriendDto> Friends { get; set; }
    }

    [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/HangmanGameEntities.Dtos")]
    public class SendFriendRequestByEmailDto
    {
        [DataMember]
        public int SenderUserId { get; set; }

        [DataMember]
        public string ReceiverEmail { get; set; }
    }

    [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/HangmanGameEntities.Dtos")]
    public class LeaderboardPlayerDto
    {
        [DataMember]
        public int Position { get; set; }

        [DataMember]
        public int UserId { get; set; }

        [DataMember]
        public string FullName { get; set; }

        [DataMember]
        public int GlobalScore { get; set; }
    }

    [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/HangmanGameEntities.Dtos")]
    public class LeaderboardOperationResultDto
    {
        [DataMember]
        public bool Success { get; set; }

        [DataMember]
        public string Message { get; set; }

        [DataMember]
        public List<LeaderboardPlayerDto> TopPlayers { get; set; }

        [DataMember]
        public LeaderboardPlayerDto CurrentPlayerRank { get; set; }
    }

    // ── Service contracts ─────────────────────────────────────────────────────

    [ServiceContract]
    public interface IUserService
    {
        [OperationContract] OperationResultDto RegisterUser(RegisterUserDto registerUserDto);
        [OperationContract] OperationResultDto Login(LoginDto loginDto);
        [OperationContract] OperationResultDto GetUserProfile(int userId);
        [OperationContract] OperationResultDto UpdateUserProfile(UpdateUserProfileDto updateUserProfileDto);
    }

    [ServiceContract]
    public interface ICategoryService
    {
        [OperationContract] GameOperationResultDto GetAllCategories(string languageCode);
        [OperationContract] GameOperationResultDto GetWordsByCategory(int categoryId, string languageCode);
    }

    public interface IGameCallback
    {
        [OperationContract(IsOneWay = true)] void OnGameStateChanged(GameStateDto gameState);
        [OperationContract(IsOneWay = true)] void OnPlayerJoined(GameDto game);
        [OperationContract(IsOneWay = true)] void OnGameEnded(GameStateDto gameState);
    }

    [ServiceContract(CallbackContract = typeof(IGameCallback), SessionMode = SessionMode.Required)]
    public interface IGameService
    {
        [OperationContract] GameOperationResultDto CreateGame(CreateGameDto createGameDto);
        [OperationContract] GameOperationResultDto JoinGame(int gameId, int retadorId);
        [OperationContract] GameOperationResultDto GetAvailableGames();
        [OperationContract] GameOperationResultDto GetGameState(int gameId);
        [OperationContract] GameOperationResultDto GuessLetter(GuessLetterDto guessLetterDto);
        [OperationContract] GameOperationResultDto AbandonGame(int gameId, int userId);
        [OperationContract] GameOperationResultDto GetUserScore(int userId);
        [OperationContract(IsOneWay = true)] void RegisterForGame(int gameId, int userId);
    }

    [ServiceContract]
    public interface IFriendService
    {
        [OperationContract] FriendOperationResultDto SendFriendRequest(SendFriendRequestDto requestDto);

        [OperationContract] FriendOperationResultDto GetPendingFriendRequests(int userId);

        [OperationContract] FriendOperationResultDto GetSentFriendRequests(int userId);

        [OperationContract] FriendOperationResultDto AcceptFriendRequest(RespondFriendRequestDto requestDto);

        [OperationContract] FriendOperationResultDto RejectFriendRequest(RespondFriendRequestDto requestDto);

        [OperationContract] FriendOperationResultDto CancelFriendRequest(RespondFriendRequestDto requestDto);

        [OperationContract] FriendOperationResultDto RemoveFriend(RemoveFriendDto requestDto);

        [OperationContract] FriendOperationResultDto GetFriends(int userId);

        [OperationContract]
        FriendOperationResultDto SendFriendRequestByEmail(SendFriendRequestByEmailDto requestDto);
    }

    [ServiceContract]
    public interface ILeaderboardService
    {
        [OperationContract]
        LeaderboardOperationResultDto GetTopScoreLeaderboard(int currentUserId);
    }
}
