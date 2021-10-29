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
            ClientPrincipal clientPrincipal = principal.ToClientPrincipal();


            try
            {
                var principalJson = JsonSerializer.Serialize(clientPrincipal, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                //var p = principal.ToString();
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
