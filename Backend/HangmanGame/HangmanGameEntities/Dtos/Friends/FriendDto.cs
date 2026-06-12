using System;
using System.Runtime.Serialization;

namespace HangmanGameEntities.Dtos
{
    [DataContract]
    public class FriendDto
    {
        [DataMember]
        public int UserId { get; set; }

        [DataMember]
        public string FullName { get; set; }

        [DataMember]
        public string Email { get; set; }

        [DataMember]
        public int GlobalScore { get; set; }

        [DataMember]
        public DateTime FriendsSince { get; set; }
    }
}