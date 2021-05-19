using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using protecta.WC1.api.DTO;
using protecta.WC1.api.Services;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public ActionResult Create(RequestWC1DTO item)
        {
            return Ok(new WC1Service().Create(item));
        }

        [Route("resolution")]
        [HttpPost]
        public ActionResult Resolution(RequestWC1ResolutionDTO item)
        {

            return Ok(new WC1Service().Resolution(item));
        }
    }
}
