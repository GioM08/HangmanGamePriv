using System.Runtime.Serialization;

namespace HangmanGameEntities.Dtos
{
    [DataContract]
    public class SendFriendRequestDto
    {
        [DataMember]
        public int SenderUserId { get; set; }

        [DataMember]
        public int ReceiverUserId { get; set; }
    }
}