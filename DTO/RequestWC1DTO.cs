using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace protecta.WC1.api.DTO
{
    [Serializable]
    [DataContract]
    public class RequestWC1DTO
    {
        [DataMember]
        public string dDate { get; set; }
        [DataMember]
        public RequestWc1 item { get; set; }
    }
}
