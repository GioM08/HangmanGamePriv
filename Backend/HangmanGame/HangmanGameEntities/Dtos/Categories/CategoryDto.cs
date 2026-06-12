using System.Runtime.Serialization;

namespace HangmanGameEntities.Dtos
{
    [DataContract]
    public class CategoryDto
    {
        [DataMember] public int CategoryId { get; set; }
        [DataMember] public string Name { get; set; }
        [DataMember] public string LanguageCode { get; set; }
    }
}
