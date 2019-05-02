using FindLostThingsBackEnd.Middleware;
using FindLostThingsBackEnd.Model.Request.User;
using FindLostThingsBackEnd.Model.Response.User;
using FindLostThingsBackEnd.Persistence.Model;
using FindLostThingsBackEnd.Service.User;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Linq;

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

        [HttpPut("contacts")]
        [TypeFilter(typeof(AuthorizeACTKAttribute))]
        public JsonResult UpdateContactsInfo([FromBody] AccountContacts contacts, [FromHeader] long userid)
        {
            return new JsonResult(services.UpdateAccountContacts(contacts, userid));
        }

        [HttpPut("info")]
        [TypeFilter(typeof(AuthorizeACTKAttribute))]
        public JsonResult UpdateUserInfo([FromBody] UserInfo info)
        {
            return new JsonResult(services.UpdateUserInfo(info));
        }

        [HttpGet("info")]
        [TypeFilter(typeof(AuthorizeACTKAttribute))]
        public JsonResult GetUserInfo([FromHeader] long userid,[FromQuery] long? query)
        {
            long QueryUserID = query == null ? userid : (long)query;
            return new JsonResult(services.GetUserInfo(QueryUserID));
        }

        [HttpPut("js-info")]
        [TypeFilter(typeof(AuthorizeACTKAttribute))]
        public JsonResult UpdateUserInfo([FromBody] AuthenticatorResponseUserInfo info)
        {
            return new JsonResult(services.UpdateUserInfo(AccountIdTransformer(info)));
        }

        [HttpGet("js-info")]
        [TypeFilter(typeof(AuthorizeACTKAttribute))]
        public JsonResult GetUserInfo([FromHeader] string userid)
        {
            return new JsonResult(services.GetUserInfo(long.Parse(userid)));
        }

        [HttpGet("js-unauth")]
        [TypeFilter(typeof(AuthorizeACTKAttribute))]
        public JsonResult GetUnAuthenticatedAccountInfo()
        {
            var info = services.GetUnAuthenticatedAccountInfo();
            if (info.StatusCode == 0)
            {
                var FullInfo = info as UnAuthenticatedUserResponse;
                return new JsonResult(new
                {
                    StatusCode = info.StatusCode,
                    UserList = FullInfo.UserList.Select(x => AccountIdTransformer(x))
                });
            }
            return new JsonResult(info);
        }


        private AuthenticatorResponseUserInfo AccountIdTransformer(UserInfo x)
        {
            var Jobj = JObject.FromObject(x);
            var LongId = Jobj.SelectToken("Id").Value<long>();
            var stringId = LongId.ToString();
            Jobj.SelectToken("Id").Replace(stringId);
            return Jobj.ToObject<AuthenticatorResponseUserInfo>();
        }

        private UserInfo AccountIdTransformer(AuthenticatorResponseUserInfo x)
        {
            var Jobj = JObject.FromObject(x);
            var stringId = Jobj.SelectToken("Id").Value<string>();
            var LongId = long.Parse(stringId);
            Jobj.SelectToken("Id").Replace(LongId);
            return Jobj.ToObject<UserInfo>();
        }
    }
}
