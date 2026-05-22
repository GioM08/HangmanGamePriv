using System.Collections.Generic;
using System.Runtime.Serialization;

namespace HangmanGameEntities.Dtos
{
    [DataContract]
    public class GameStateDto
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
}
