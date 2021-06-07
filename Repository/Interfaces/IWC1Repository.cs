using protecta.WC1.api.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace protecta.WC1.api.Repository.Interfaces
{
    public interface IWC1Repository
    {
        ResponseDTO SaveResult(ResponseWc1 item, string SystemCaseId);
    }
}
