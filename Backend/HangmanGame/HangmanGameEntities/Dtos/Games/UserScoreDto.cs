using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace HangmanGameEntities.Dtos
{
    [DataContract]
    public class UserScoreDto
    {
        [DataMember] public int TotalPoints { get; set; }
        [DataMember] public int GamesPlayed { get; set; }
        [DataMember] public int GamesWon { get; set; }
        [DataMember] public int GamesLost { get; set; }
        [DataMember] public List<GameResultDto> WonGames { get; set; }
        [DataMember] public List<GameResultDto> LostGames { get; set; }
        [DataMember] public List<PenaltyDto> Penalties { get; set; }
    }

    [DataContract]
    public class GameResultDto
    {
        [DataMember] public DateTime GameDate { get; set; }
        [DataMember] public string Word { get; set; }
        [DataMember] public string OpponentName { get; set; }
        [DataMember] public string ResultType { get; set; }
        [DataMember] public int PointsEarned { get; set; }
    }

    [DataContract]
    public class PenaltyDto
    {
        [DataMember] public DateTime PenaltyDate { get; set; }
        [DataMember] public string Word { get; set; }
        [DataMember] public string Reason { get; set; }
        [DataMember] public int PointsDeducted { get; set; }
    }
}
