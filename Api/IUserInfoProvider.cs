using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace BlazorApp.Api
{
    public interface IUserInfoProvider
    {
        ClaimsPrincipal Parse(HttpRequest req);
    }
}