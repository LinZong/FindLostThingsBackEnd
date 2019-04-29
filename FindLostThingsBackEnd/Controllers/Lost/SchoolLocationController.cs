using FindLostThingsBackEnd.Service.Lost;
using Microsoft.AspNetCore.Mvc;

namespace FindLostThingsBackEnd.Controllers.Lost
{
    [Route("school/")]
    [ApiController]
    public class SchoolLocationController : ControllerBase
    {
        private readonly SchoolLocationServices schoolLocation;
        public SchoolLocationController(SchoolLocationServices sv)
        {
            schoolLocation = sv;
        }

        [HttpGet("list")]
        public JsonResult GetSupportSchools()
        {
            return new JsonResult(schoolLocation.GetSupportSchools());
        }

        [HttpGet("building")]
        public JsonResult GetSchoolBuildings([FromQuery] int id)
        {
            return new JsonResult(schoolLocation.GetSchoolBuildingInfo(id));
        }
    }
}
