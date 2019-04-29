using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using FindLostThingsBackEnd.Middleware;
using Microsoft.AspNetCore.Authorization;

namespace FindLostThingsBackEnd.Controllers
{
    [Route("api/")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        // GET api/values
        [HttpGet("values")]
        [Authorize]
        public ActionResult<string> Get()
        {
            return "It works!";
        }
    }
}
