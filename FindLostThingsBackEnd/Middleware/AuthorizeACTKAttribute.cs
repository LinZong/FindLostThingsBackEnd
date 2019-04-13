using System;
using System.Threading.Tasks;
using FindLostThingsBackEnd.Service.User;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;

namespace FindLostThingsBackEnd.Middleware
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = false)]
    public sealed class AuthorizeACTKAttribute : Attribute, IAsyncAuthorizationFilter
    {
        private readonly UserServices services;
        public AuthorizeACTKAttribute(UserServices serv)
        {
            services = serv;
        }
        public Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            //var controllerActionDescriptor = context.ActionDescriptor as ControllerActionDescriptor;
            //var attribs = controllerActionDescriptor.MethodInfo.Attributes;
            //string controllerName = controllerActionDescriptor?.ControllerName;
            //string actionName = controllerActionDescriptor?.ActionName;

            StringValues ACTK, USERID;
            bool IsACTK = context.HttpContext.Request.Headers.TryGetValue("actk", out ACTK);
            bool IsUSERID = context.HttpContext.Request.Headers.TryGetValue("userid", out USERID);
            if (!IsACTK || !IsUSERID)
            {
                context.Result = new ForbidResult("AuthorizeACTK");
            }
            else
            {
                string Actk = ACTK[0];
                string Userid = USERID[0];
                bool ParseActkOK = long.TryParse(Userid, out long LongUserID);
                if(ParseActkOK)
                {
                    if (services.ValidateACTK(LongUserID, Actk))
                    {
                        return Task.CompletedTask;
                    }
                }
                context.Result = new ForbidResult("AuthorizeACTK");
            }
            return Task.CompletedTask;
        }
    }
}