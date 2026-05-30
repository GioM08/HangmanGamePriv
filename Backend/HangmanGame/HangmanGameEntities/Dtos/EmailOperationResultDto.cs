using System.Runtime.Serialization;

namespace HangmanGameEntities.Dtos
{
    [DataContract]
    public class EmailOperationResultDto
    {
        [DataMember]
        public bool Success { get; set; }

        [DataMember]
        public string Message { get; set; }
    }
}
