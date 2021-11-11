using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging; 
using BlazorApp.Shared;
using System.Text.Json;
using System.Security.Claims;
using System.Collections.Generic;

namespace BlazorApp.Api
{
    public class UserInfo
    {
        private readonly IUserInfoProvider _userInfoProvider;

        public UserInfo(IUserInfoProvider provider) => _userInfoProvider = provider;

        [FunctionName("UserInfo")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "secured/UserInfo")] HttpRequest req,
            ILogger log, ClaimsPrincipal principal)
        {
            try
            {
                //if (!principal.Identity.IsAuthenticated)
                //{
                //    return new NotFoundResult();
                //}

                //if(!user.Identity.IsAuthenticated)
                //{
                //    return new NotFoundResult();
                //}

                var user = _userInfoProvider.Parse(req);
                var clientFromReq = user.ToClientPrincipal();

                

                ClientPrincipal clientPrincipal = principal.ToClientPrincipal();

                var principals = new List<ClientPrincipal>() { clientFromReq, clientPrincipal };
                var principalJson = JsonSerializer.Serialize(principals, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }); 
                return new OkObjectResult(principalJson);
            }
            catch (Exception ex)
            {
                log.LogError(ex, "UserInfo failure");
                return new NotFoundResult();
            }
        }
    }
}
