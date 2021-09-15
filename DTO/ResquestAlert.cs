using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace protecta.WC1.api.DTO
{
    [Serializable]
    [DataContract]
    public class ResquestAlert
    {
        [DataMember]
        public string name { get; set; }
        [DataMember]
        public string typeDocument { get; set; }
        [DataMember]
        public string idDocNumber { get; set; }
        [DataMember]
        public string periodId { get; set; }
        [DataMember]
        public string alertId { get; set; }
        [DataMember]
        public string tipoCargaId { get; set; }
        [DataMember]
        public string sClient { get; set; }
        [DataMember]
        public string nIdUsuario { get; set; }
    }
}