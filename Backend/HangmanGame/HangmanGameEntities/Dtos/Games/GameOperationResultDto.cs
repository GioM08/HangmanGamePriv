using System.Collections.Generic;
using System.Runtime.Serialization;

namespace HangmanGameEntities.Dtos
{
    [DataContract]
    public class GameOperationResultDto
    {
        [DataMember] public bool Success { get; set; }
        [DataMember] public string MessageKey { get; set; }
        [DataMember] public string Message { get; set; }
        [DataMember] public GameDto Game { get; set; }
        [DataMember] public GameStateDto GameState { get; set; }
        [DataMember] public List<GameDto> Games { get; set; }
        [DataMember] public List<WordDto> Words { get; set; }
        [DataMember] public List<CategoryDto> Categories { get; set; }
        [DataMember] public UserScoreDto UserScore { get; set; }
    }
}
