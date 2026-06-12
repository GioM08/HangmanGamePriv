using System;
using System.Runtime.Serialization;

namespace HangmanGameEntities.Dtos
{
    [DataContract]
    public class UpdateUserProfileDto
    {
        [DataMember]
        public int UserId { get; set; }

        [DataMember]
        public string FullName { get; set; }

        [DataMember]
        public DateTime BirthDate { get; set; }

        [DataMember]
        public string PhoneNumber { get; set; }
    }
}