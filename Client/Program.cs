using System;
using System.Net.Http;
using System.Threading.Tasks; 
using BlazorUtils.StaticExtentions;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;

namespace BlazorApp.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");

            var baseAddress = builder.Configuration["BaseAddress"] ?? builder.HostEnvironment.BaseAddress;
            builder.Services.AddScoped(_ => new HttpClient { BaseAddress = new Uri(baseAddress) });

           
            builder.Services.AddStaticWebAppsAuthentication();
            builder.Services.AddMudServices().AddLocalization(options =>
            {
                options.ResourcesPath = "Resources";                
            });


            await builder.Build().RunAsync();
        }
    }
}