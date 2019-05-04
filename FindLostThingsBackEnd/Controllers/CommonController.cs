using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using FindLostThingsBackEnd.Persistence.DAO.Context;
using System.Linq;

namespace FindLostThingsBackEnd.Controllers
{
    [Route("common/")]
    [ApiController]
    public class CommonController : ControllerBase
    {
        private readonly LostContext context;
        public CommonController(LostContext ctx)
        {
            context = ctx;
        }
        // GET api/values
        [HttpGet("works")]
        public ActionResult<string> Get()
        {
            return "It works!";
        }

        [HttpGet("update")]
        public JsonResult GetLatestVersion()
        {
            return new JsonResult(context.Version.FirstOrDefault());
        }
    }
}
