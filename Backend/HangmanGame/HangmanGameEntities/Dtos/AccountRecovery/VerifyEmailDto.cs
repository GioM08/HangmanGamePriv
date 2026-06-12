using System.Runtime.Serialization;

namespace HangmanGameEntities.Dtos
{
    [DataContract]
    public class VerifyEmailDto
    {
        [DataMember]
        public int UserId { get; set; }

        [DataMember]
        public string Code { get; set; }
    }
}
