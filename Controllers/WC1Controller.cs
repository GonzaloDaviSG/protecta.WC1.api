using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using protecta.WC1.api.DTO;
using protecta.WC1.api.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace protecta.WC1.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WC1Controller : Controller
    {
        // GET: WC1Controller/Details/5
        [Route("GetDemandaSearch")]
        [HttpPost]
        public async Task<ResponseDTO> GetDemandaSearch(ResquestAlert item)
        {
            ResponseDTO response = new ResponseDTO();
            Task<ResponseDTO> taskA = null;
            try
            {
                taskA = Task.Run(() =>
                {
                    return new WC1Service().GetDemandaSearch(item);
                });
                taskA.Wait();

            }
            catch (Exception ex)
            {
                taskA = Task.Run(() =>
                {
                    response.sMessage = "Comuniquese con el Administrador";
                    response.sStatus = "ERROR";
                    response.nCode = 1;
                    return response;
                });
            }
            response = taskA.Result;
            response.code = response.nCode;
            response.mensaje = response.sMessage.ToString();
            return taskA.Result;

        }
        [Route("GetClients")]
        [HttpPost]
        public async Task<ResponseDTO> getClients(ResquestAlert item)
        {
            ResponseDTO response = new ResponseDTO();
            Task<ResponseDTO> taskA = null;
            try
            {
                taskA = Task.Run(async ()  =>
                {
                    return await new WC1Service().getClients(item);
                });
                taskA.Wait();
            }
            catch (Exception ex)
            {
                taskA = Task.Run(() =>
                {
                    response.sMessage = "Comuniquese con el Administrador";
                    response.sStatus = "ERROR";
                    response.nCode = 1;
                    return response;
                });
            }
            return taskA.Result;

        }
        [Route("getClientsAutomatic")]
        [HttpPost]
        public async Task<ResponseDTO> getClientsAutomatic(ResquestAlert item)
        {
            ResponseDTO response = new ResponseDTO();
            Task<ResponseDTO> taskA = null;
            try
            {
                taskA = Task.Run(async () =>
                {
                    return await new WC1Service().getClientsAutomatic(item);
                });
                taskA.Wait();
            }
            catch (Exception ex)
            {
                taskA = Task.Run(() =>
                {
                    response.sMessage = "Comuniquese con el Administrador";
                    response.sStatus = "ERROR";
                    response.nCode = 1;
                    return response;
                });
            }
            return taskA.Result;

        }

        [Route("getCoincidenceNotPep")]
        [HttpPost]
        public async Task<ListResponseDTO> getCoincidenceNotPep(ResquestAlert item)
        {
            return await new WC1Service().getCoincidenceNotPep(item);
        }
        [Route("deleteCases")]
        [HttpGet]
        public async Task<IActionResult> deteleCases()
        {
            return Ok(await new WC1Service().deleteCases());
        }
    }
}
