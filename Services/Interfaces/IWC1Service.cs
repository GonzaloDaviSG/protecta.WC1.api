
using protecta.WC1.api.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace protecta.WC1.api.Services.Interfaces
{
    public interface IWC1Service
    {
        ResponseWc1 Create(RequestWc1 item);

    }
}
