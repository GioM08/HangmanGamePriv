using System.Collections.Generic;
using System.Runtime.Serialization;

namespace HangmanGameEntities.Dtos
{
    [DataContract]
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
}
