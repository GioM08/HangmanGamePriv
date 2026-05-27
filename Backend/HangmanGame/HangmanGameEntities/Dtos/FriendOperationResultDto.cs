using System.Collections.Generic;
using System.Runtime.Serialization;

namespace HangmanGameEntities.Dtos
{
    [DataContract]
    public class FriendOperationResultDto
    {
        [DataMember]
        public bool Success { get; set; }

        [DataMember]
        public string Message { get; set; }

        [DataMember]
        public FriendRequestDto FriendRequest { get; set; }

        [DataMember]
        public List<FriendRequestDto> FriendRequests { get; set; }

        [DataMember]
        public List<FriendDto> Friends { get; set; }
    }
}