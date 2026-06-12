using System.Runtime.Serialization;

namespace HangmanGameEntities.Dtos
{
    [DataContract]
    public class RemoveFriendDto
    {
        [DataMember]
        public int CurrentUserId { get; set; }

        [DataMember]
        public int FriendUserId { get; set; }
    }
}
