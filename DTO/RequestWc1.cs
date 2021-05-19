using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace protecta.WC1.api.DTO
{
    [Serializable]
    [DataContract]
    public class RequestWc1
    {
        [DataMember]
        public string name { get; set; }
        //[DataMember]
        //public bool nameTransposition { get; set; }
        [DataMember]
        public string entityType { get; set; }
        [DataMember]
        public List<string> providerTypes { get; set; }
        [DataMember]
        public string groupId { get; set; }
        [DataMember]
        public List<Properties> secondaryFields { get; set; }
        [DataMember]
        public List<Properties> customFields { get; set; }
    }
    [Serializable]
    [DataContract]
    public class Properties {
        [DataMember]
        public string typeId { get; set; }
        [DataMember]
        public string value { get; set; }
    }
}