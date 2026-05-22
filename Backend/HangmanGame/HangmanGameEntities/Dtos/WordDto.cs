using System.Runtime.Serialization;

namespace HangmanGameEntities.Dtos
{
    [DataContract]
    public class WordDto
    {
        [DataMember] public int WordId { get; set; }
        [DataMember] public string Text { get; set; }
        [DataMember] public string Hint { get; set; }
        [DataMember] public int CategoryId { get; set; }
        [DataMember] public string CategoryName { get; set; }
        [DataMember] public int Difficulty { get; set; }
    }
}
