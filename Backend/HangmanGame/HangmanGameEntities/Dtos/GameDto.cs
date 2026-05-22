using System;
using System.Runtime.Serialization;

namespace HangmanGameEntities.Dtos
{
    [DataContract]
    public class GameDto
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
}
