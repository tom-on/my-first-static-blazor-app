using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace BlazorApp.Shared
{
    public class WeatherForecast
    {
        public DateTime Date { get; set; }

        public int TemperatureC { get; set; }

        public string Summary { get; set; }

        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
    }


    public class ClientPrincipal
    {
        public string IdentityProvider { get; set; }
        public string UserId { get; set; }
        public string UserDetails { get; set; }
        public IEnumerable<string> UserRoles { get; set; } = new List<string>();

        public IEnumerable<ClientClaim> Claims { get; set; } = new List<ClientClaim>();
    }

    public class ClientClaim
    {
        public string Type { get; set; }
        public string Value { get; set; }
        public ClientClaim(string type, string value) => (Type, Value) = (type, value);

    }


    public static class ClientPrincipalExt
    {
        public static ClaimsPrincipal ToClaimsPrincipal(this ClientPrincipal principal)
        {
            principal.UserRoles = principal.UserRoles?.Except(new string[] { "anonymous" }, StringComparer.CurrentCultureIgnoreCase);

            //if (!principal.UserRoles?.Any() ?? true)
            //{
            //    return new ClaimsPrincipal();
            //}

            var identity = new ClaimsIdentity(principal.IdentityProvider);
            if (!string.IsNullOrEmpty(principal.UserId))
            {
                identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, principal.UserId));
            }
            if (!string.IsNullOrEmpty(principal.UserDetails))
            {
                identity.AddClaim(new Claim(ClaimTypes.Name, principal.UserDetails));
            }
            if (principal.UserRoles?.Any() == true)
            {
                identity.AddClaims(principal.UserRoles.Select(r => new Claim(ClaimTypes.Role, r)));
            }
            if (principal.Claims.Any())
            {
                identity.AddClaims(principal.Claims.Select(c => new Claim(c.Type, c.Value)).ToList());
            }

            return new ClaimsPrincipal(identity);
        }
    }

    public static class ClaimsPrincipalExt
    {
        public static ClientPrincipal ToClientPrincipal(this ClaimsPrincipal principal)
        {
            ClientPrincipal clientPrincipal = new ClientPrincipal
            {
                IdentityProvider = principal.Identity.AuthenticationType,
                UserId = principal.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).FirstOrDefault()?.Value,
                UserDetails = principal.Claims.Where(c => c.Type == ClaimTypes.Name).FirstOrDefault()?.Value,
                UserRoles = principal.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToList(),
                Claims = principal.Claims.Select(c => new ClientClaim(c.Type, c.Value)).ToList()
            };
            return clientPrincipal;
        }
    }
}
