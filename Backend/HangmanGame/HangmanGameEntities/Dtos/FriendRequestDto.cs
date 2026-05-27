using System;
using System.Runtime.Serialization;

namespace HangmanGameEntities.Dtos
{
    [DataContract]
    public class FriendRequestDto
    {
        [DataMember]
        public int FriendRequestId { get; set; }

        [DataMember]
        public int SenderUserId { get; set; }

        [DataMember]
        public string SenderFullName { get; set; }

        [DataMember]
        public string SenderEmail { get; set; }

        [DataMember]
        public int ReceiverUserId { get; set; }

        [DataMember]
        public string ReceiverFullName { get; set; }

        [DataMember]
        public string ReceiverEmail { get; set; }

        [DataMember]
        public int Status { get; set; }

        [DataMember]
        public DateTime CreatedAt { get; set; }
    }
}