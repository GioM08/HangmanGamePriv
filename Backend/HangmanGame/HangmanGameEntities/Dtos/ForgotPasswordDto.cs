using System.Runtime.Serialization;

namespace HangmanGameEntities.Dtos
{
    [DataContract]
    public class ForgotPasswordDto
    {
        [DataMember]
        public string Email { get; set; }
    }
}
