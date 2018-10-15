using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityFramework.Models;
using IdentityFramework.Service;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityFramework.API
{
    public static class ServiceExtensions
    {
        public static IServiceCollection RegisterMyServices(this IServiceCollection services)
        {
            services.AddTransient<IUserService, UserService>();
            services.AddSingleton<IEmailService, EmailService>();
            
            // Add all other services here.
            return services;
        }
    }
}
