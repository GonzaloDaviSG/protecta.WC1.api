using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace protecta.WC1.api.DTO
{
    [DataContract]
    [Serializable]
    public class ResponseCoincidenciaDemanda
    {
        [DataMember]
        public string mensaje { get; set; }
        [DataMember]
        public string sMessage { get; set; }
        [DataMember]
        public int code { get; set; }
        [DataMember]
        public int nCode { get; set; }
        [DataMember]
        public List<CoincidenciaDemanda> Items { get; set; }
        [DataMember]
        public string mensajeError { get; set; }
    }
    public class CoincidenciaDemanda
    {
        [DataMember]
        public string DFECHA_BUSQUEDA { get; set; }
        [DataMember]
        public string SCARGO { get; set; }
        [DataMember]
        public int NIDTIPOLISTA { get; set; }
        [DataMember]
        public string SDESTIPOLISTA { get; set; }
        [DataMember]
        public string SNOMBRE_BUSQUEDA { get; set; }
        [DataMember]
        public string SNOMBRE_COMPLETO { get; set; }
        [DataMember]
        public string SNOMBRE_TERMINO { get; set; }
        [DataMember]
        public string SNUM_DOCUMENTO { get; set; }
        [DataMember]
        public string SPORCEN_COINCIDENCIA { get; set; }
        [DataMember]
        public string SPORCEN_SCORE { get; set; }
        [DataMember]
        public string STIPO_DOCUMENTO { get; set; }
        [DataMember]
        public string STIPO_PERSONA { get; set; }
        [DataMember]
        public string SUSUARIO_BUSQUEDA { get; set; }
        [DataMember]
        public int NIDPROVEEDOR { get; set; }
        [DataMember]
        public string SDESPROVEEDOR { get; set; }
        [DataMember]
        public string STIPOCOINCIDENCIA { get; set; }
        [DataMember]
        public string SNUMDOC_BUSQUEDA { get; set; }
        [DataMember]
        public string SCOINCIDENCIA { get; set; }
    }
}
