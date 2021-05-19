using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace protecta.WC1.api.DTO
{
    [Serializable]
    [DataContract]
    public class RequestWC1ResolutionDTO
    {
        [DataMember]
        public string caseSystemId { get; set; }
        [DataMember]
        public Resolution item {get; set;}
    }

    public class Resolution
    {
        [DataMember]
        public List<string> resultIds { get; set; }
        [DataMember]
        public string statusId { get; set; }
        [DataMember]
        public string riskId { get; set; }
        [DataMember]
        public string reasonId { get; set; }
        [DataMember]
        public string resolutionRemark { get; set; }
    }
}
