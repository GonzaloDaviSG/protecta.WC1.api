using System;
using System.Collections.Generic;
using System.Data;
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
        public string grupoSenalId { get; set; }
        [DataMember]
        public string grupoSubSenalId { get; set; }

        [DataMember]
        public string name { get; set; }
        [DataMember]
        public string tipo { get; set; }
        [DataMember]
        public string typeDocument { get; set; }
        [DataMember]
        public string idDocNumber { get; set; }
        [DataMember]
        public string tipoSex { get; set; }
        [DataMember]
        public string birthday { get; set; }
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
        [DataMember]
        public int tipoBusqueda { get; set; }
        [DataMember]
        public string codBusqueda { get; set; }
        [DataMember]
        public List<Dictionary<string, string>> items  { get; set; }
    }
}