using FindLostThingsBackEnd.Middleware;
using FindLostThingsBackEnd.Model.Response;
using FindLostThingsBackEnd.Persistence.Model;
using FindLostThingsBackEnd.Service.Lost;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FindLostThingsBackEnd.Controllers.Lost
{
    [Route("thing/")]
    [ApiController]
    public class ThingController : ControllerBase
    {
        private ThingServices thingServices;
        public ThingController(ThingServices ts)
        {
            thingServices = ts;
        }

        [HttpGet("list")]
        [TypeFilter(typeof(AuthorizeACTKAttribute))]
        public JsonResult GetThingsTimeLine([FromForm] string EndItemId, [FromForm] string HaveFetchedItemCount, [FromForm] int Count)
        {
            int RHaveFetchedItemCount = 0;
            int.TryParse(HaveFetchedItemCount, out RHaveFetchedItemCount);
            if (RHaveFetchedItemCount < 0) RHaveFetchedItemCount = 0;
            return new JsonResult(thingServices.GetTimeLines(RHaveFetchedItemCount, EndItemId, Count));
        }

        [HttpPost("publish")]
        [TypeFilter(typeof(AuthorizeACTKAttribute))]
        public JsonResult PublishLostThing([FromBody] LostThingsRecord record)
        {

            return new JsonResult(thingServices.PublishLostThingRecord(record));
        }

        [HttpGet("mylist")]
        [TypeFilter(typeof(AuthorizeACTKAttribute))]
        public JsonResult GetMyThingsList([FromForm] int type,[FromHeader] long userid)
        {
            long UserID = userid;
            switch (type)
            {
                case 0:
                    return new JsonResult(thingServices.GetUserThings(UserID, UserThingsListType.Given));
                case 1:
                    return new JsonResult(thingServices.GetUserThings(UserID, UserThingsListType.Published));
                default:
                    return new JsonResult(thingServices.GetUserThings(UserID, UserThingsListType.Published));
            }
        }

        [HttpPost("update")]
        [TypeFilter(typeof(AuthorizeACTKAttribute))]
        public JsonResult UpdateLostThingInfo([FromBody] LostThingsRecord record)
        {
            return new JsonResult(thingServices.UpdateLostThingRecord(record));
        }

    }
}
