using FindLostThingsBackEnd.Middleware;
using FindLostThingsBackEnd.Model.Request.User;
using FindLostThingsBackEnd.Service.User;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FindLostThingsBackEnd.Controllers.User
{
    [Route("user/")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private UserServices services;
        public UserController(UserServices serv)
        {
            services = serv;
        }

        [HttpPost("login")]
        public JsonResult UploadAccountInfo([FromBody] LoginUploadAccountInfo info)
        {
            return new JsonResult(services.ProcessLoginAccountInfo(info));
        }

        [HttpPut("info")]
        [TypeFilter(typeof(AuthorizeACTKAttribute))]
        public JsonResult UpdateContactsInfo([FromBody] AccountContacts contacts,[FromHeader] long userid)
        {
            return new JsonResult(services.UpdateAccountContacts(contacts, userid));
        }
        [HttpGet("info")]
        [TypeFilter(typeof(AuthorizeACTKAttribute))]
        public JsonResult GetUserInfo([FromHeader] long userid)
        {
            return new JsonResult(services.GetUserInfo(userid));
        }
    }
}
