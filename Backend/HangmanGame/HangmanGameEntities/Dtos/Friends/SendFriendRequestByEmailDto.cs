using System.Runtime.Serialization;

namespace HangmanGameEntities.Dtos
{
    [DataContract]
    public class SendFriendRequestByEmailDto
    {
        [DataMember]
        public int SenderUserId { get; set; }

        [DataMember]
        public string ReceiverEmail { get; set; }
    }
}