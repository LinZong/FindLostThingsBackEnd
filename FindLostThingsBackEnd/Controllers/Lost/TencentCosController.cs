using FindLostThingsBackEnd.Middleware;
using FindLostThingsBackEnd.Model.Response;
using FindLostThingsBackEnd.Model.Response.Lost;
using FindLostThingsBackEnd.Service.Tencent;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FindLostThingsBackEnd.Controllers.Lost
{

    [Route("tencent/")]
    [ApiController]
    public class TencentCosController : ControllerBase
    {
        private TencentCosTempKey keyService;
        public TencentCosController(TencentCosTempKey keyServ)
        {
            keyService = keyServ;
        }

        [HttpGet("coskey")]
        [TypeFilter(typeof(AuthorizeACTKAttribute))]
        public JsonResult GetTempCosAccessKey()
        {
            var Resp = keyService.GetSignature();
            if(Resp.IsSuccessful)
            {
                var RespJObj = JObject.Parse(Resp.Content);
                if (RespJObj.TryGetValue("Response", out JToken RespToken))
                {
                    return new JsonResult(new TencentCosTempKeyResponse()
                    {
                        StatusCode = 0,
                        Region = keyService.Region,
                        FullBucketName = $"{keyService.BucketName}-{keyService.AppID}",
                        Response = RespToken
                    });
                }
                return new JsonResult(new CommonResponse()
                {
                    StatusCode = 1302
                });
            }
            return new JsonResult(new CommonResponse()
            {
                StatusCode = 1301
            });
            
        }
    }
}
