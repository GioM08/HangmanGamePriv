using System.Runtime.Serialization;

namespace HangmanGameEntities.Dtos
{
    [DataContract]
    public class GuessLetterDto
    {
        [DataMember] public int GameId { get; set; }
        [DataMember] public int UserId { get; set; }
        [DataMember] public string Letter { get; set; }
    }
}
