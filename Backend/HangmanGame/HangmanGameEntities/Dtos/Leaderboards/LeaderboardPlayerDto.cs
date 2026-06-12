using System.Runtime.Serialization;

namespace HangmanGameEntities.Dtos
{
    [DataContract]
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
}
