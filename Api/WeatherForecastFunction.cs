using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

using BlazorApp.Shared;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace BlazorApp.Api
{
    public class WeatherForecastFunction
    {
        private readonly IUserInfoProvider _userInfoProvider;

        public WeatherForecastFunction(IUserInfoProvider provider) => _userInfoProvider = provider;

        private static string GetSummary(int temp)
        {
            var summary = "Mild";

            if (temp >= 32)
            {
                summary = "Hot";
            }
            else if (temp <= 16 && temp > 0)
            {
                summary = "Cold";
            }
            else if (temp <= 0)
            {
                summary = "Freezing";
            }

            return summary;
        }

        [FunctionName("WeatherForecast")] 
        public IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            ClaimsPrincipal principal = _userInfoProvider.Parse(req);
            var randomNumber = new Random();
            var temp = 0;
            var roles = principal.Claims.Where(e => e.Type == ClaimTypes.Role).Select(e => e.Value);

            var result = Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = temp = randomNumber.Next(-20, 55),
                Summary = $"{GetSummary(temp)} - User: {principal.Identity.Name}; Roles count: {roles.Count()}, roles for user: {string.Concat(roles.Select(r => r))}"
            }).ToArray();

            return new OkObjectResult(result);
        } 
    }


    public class UserInfoProvider:IUserInfoProvider
    {

        public ClaimsPrincipal Parse(HttpRequest req)
        {
            var principal = new ClientPrincipal();

            if (req.Headers.TryGetValue("x-ms-client-principal", out var header))
            {
                var data = header[0];
                var decoded = Convert.FromBase64String(data);
                var json = Encoding.UTF8.GetString(decoded);
                principal = JsonSerializer.Deserialize<ClientPrincipal>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }

            return principal.ToClaimsPrincipal();
        } 
    }
}
