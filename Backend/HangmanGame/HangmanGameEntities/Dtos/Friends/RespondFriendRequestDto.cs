using System.Runtime.Serialization;

namespace HangmanGameEntities.Dtos
{
    [DataContract]
    public class RespondFriendRequestDto
    {
        [DataMember]
        public int FriendRequestId { get; set; }

        [DataMember]
        public int CurrentUserId { get; set; }
    }
}