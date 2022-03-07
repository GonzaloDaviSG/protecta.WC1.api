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

        public ActionResult Index()
        {
            return View();
        }

        // GET: WC1Controller/Details/5
        [Route("create")]
        [HttpPost]
        public ActionResult Create(RequestWc1 item)
        {
            return Ok(new WC1Service().Create(item));
        }

        [Route("resolution")]
        [HttpPost]
        public ActionResult Resolution(RequestWC1ResolutionDTO item)
        {

            return Ok(new WC1Service().Resolution(item));
        }

        [Route("listcountry")]
        [HttpGet]
        public ActionResult listCountry()
        {
            return Ok(new WC1Service().listCountry());
        }

        [Route("listnationalities")]
        [HttpGet]
        public ActionResult listNationalities()
        {
            return Ok(new WC1Service().cargaMassive());
        }

        [Route("cargamassive")]
        [HttpGet]
        public ActionResult cargaMassive()
        {
            return Ok(new WC1Service().cargaMassive());
        }

        [Route("proccessmassive")]
        [HttpGet]
        public ActionResult procesocoincidencia()
        {
            return Ok(new WC1Service().procesoCoincidencia());
        }

     
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
        [Route("GetCoincidenceMassive")]
        [HttpPost]
        public async Task<ResponseDTO> GetCoincidenceMassive(ResquestAlert item)
        {
            ResponseDTO response = new ResponseDTO();
            Task<ResponseDTO> taskA = null;
            try
            {
                taskA = Task.Run(() =>
                {
                    return new WC1Service().GetCoincidenceMassive(item);
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

        [Route("getprofiles")]
        [HttpGet("getprofiles/{referenceId}")]
        public ActionResult getProfiles(string referenceId)
        {
            return Ok(new WC1Service().getProfiles(referenceId));
        }
        [Route("validarDni")]
        [HttpGet("validarDni/{dni}")]
        public ActionResult validarDni(string dni)
        {
            return Ok(new WC1Service().validarDni(dni));
        }
        //[Route("getPrueba")]
        //[HttpGet]
        //public ActionResult Prueba()
        //{
        //    return Ok(new WC1Service().getprueba());
        //}
    }
}
