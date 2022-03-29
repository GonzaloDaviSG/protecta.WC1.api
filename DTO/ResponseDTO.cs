using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace protecta.WC1.api.DTO
{
    public class ResponseDTO
    {
        public int nId { get; set; }
        public int nCode {get;set;}
        public int code { get; set; }
        public string sMessage { get; set; }
        public string mensaje { get; set; }
        public string sStatus { get; set; }
        public List<string> data { get; set; }
        public Dictionary<string, dynamic> informacionComplementaria { get; set; }
        public List<CoincidenciaDemanda> Items { get; set; }
    }
}
