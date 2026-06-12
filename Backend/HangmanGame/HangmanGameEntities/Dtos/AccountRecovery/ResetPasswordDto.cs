using System.Runtime.Serialization;

namespace HangmanGameEntities.Dtos
{
    [DataContract]
    public class ResetPasswordDto
    {
        [DataMember]
        public string Email { get; set; }

        [DataMember]
        public string Code { get; set; }

        [DataMember]
        public string NewPassword { get; set; }
    }
}
