using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityFramework.Models;
using IdentityFramework.Service;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace IdentityFramework.API
{
    public static class ServiceExtensions
    {

        public static IServiceCollection RegisterMyServices(this IServiceCollection services, IConfiguration config)
        {
            try
            {
                var baseUrl = config["IdentityFrameworkSettings:BaseUrl"];
                Uri endPoint = new Uri(baseUrl); // this is the endpoint HttpClient will hit
                HttpClient httpClient = new HttpClient()
                {
                    BaseAddress = endPoint,
                };

                ServicePointManager.FindServicePoint(endPoint).ConnectionLeaseTimeout = 60000; // sixty seconds

                // Add all other services here.
                services.AddSingleton<HttpClient>(httpClient);
                services.AddTransient<IUserService, UserService>();
                services.AddSingleton<IEmailService, EmailService>();

                
                return services;
            }
            catch (Exception)
            {

                throw;
            }
            
        }
    }
}
