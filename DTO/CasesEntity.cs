using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace protecta.WC1.api.DTO
{
    [DataContract]
    [Serializable]
        
    public class CasesEntity
    {
       [DataMember]
        public string caseSystemId { get; set; }
        [DataMember]
        public string caseId { get; set; }

        public bool isFinish { get; set; }

    }
}
