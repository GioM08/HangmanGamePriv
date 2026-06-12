using System.Runtime.Serialization;

namespace HangmanGameEntities.Dtos
{
    [DataContract]
    public class CreateGameDto
    {
        [DataMember] public int CreatorId { get; set; }
        [DataMember] public int WordId { get; set; }
        [DataMember] public string Description { get; set; }
    }
}
