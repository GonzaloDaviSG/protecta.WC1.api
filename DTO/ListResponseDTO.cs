using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace protecta.WC1.api.DTO
{
    public class ListResponseDTO
    {
        public bool isOtherList {get;set;}
        public bool isPep { get; set; }
        public bool isIdNumber { get; set; }
        public List<DataEntity> Data { get; set; }

        public bool isError { get; set; }
        public string messageError { get; set; }
    }

    public class DataEntity
    {
        public string name { get; set; }
        public string documentType { get; set; }
        public string documentNumber { get; set; }
        public int percentage { get; set; }
    }
}
