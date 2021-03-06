﻿using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace FindLostThingsBackEnd.Middleware
{
    public class AuthorizeACTKHandler : AuthenticationHandler<AuthorizeACTKSchemeOptions>
    {

        public AuthorizeACTKHandler(IOptionsMonitor<AuthorizeACTKSchemeOptions> options,
                                    ILoggerFactory logger,
                                    UrlEncoder encoder,
                                    ISystemClock clock) : base(options, logger, encoder, clock)
        {

        }
        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            return await Task.FromResult(AuthenticateResult.NoResult());
        }

        protected override Task HandleForbiddenAsync(AuthenticationProperties properties)
        {
            Context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            //Context.Response.OnStarting(() =>
            //{
            //    Context.Response.Body.Write(Encoding.UTF8.GetBytes("Request parameter is invalid."));
            //    return Task.CompletedTask;
            //});
            return base.HandleForbiddenAsync(properties);
        }
    }
}
