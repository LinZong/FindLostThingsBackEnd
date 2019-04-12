using FindLostThingsBackEnd.Model.Response;
using FindLostThingsBackEnd.Service.Lost;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FindLostThingsBackEnd.Controllers.Lost
{
    [Route("category/")]
    [ApiController]

    public class CategoryController : ControllerBase
    {
        private readonly CategoryServices _category;
        public CategoryController(CategoryServices category)
        {
            _category = category;
        }

        [HttpGet]
        public JsonResult GetLostThingsCategory()
        {
            return new JsonResult(_category.GetThingsCategory());
        }

        [HttpGet("detail")]
        public JsonResult GetLostThingsCategoryDetail([FromQuery] int? id)
        {
            if (id == null)
            {
                return new JsonResult(new CommonResponse() { StatusCode = -1 });
            }
            return new JsonResult(_category.GetThingsCategoryDetail((int)id));
        }
    }
}
