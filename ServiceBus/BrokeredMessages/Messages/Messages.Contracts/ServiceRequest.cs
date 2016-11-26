using System.Runtime.Serialization;

namespace Messages.Contracts
{
    [DataContract]
    public class ServiceRequest
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public string Type { get; set; }
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public int CustomerId { get; set; }
    }
}
