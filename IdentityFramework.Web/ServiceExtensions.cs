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

namespace IdentityFramework.Web
{
    public static class ServiceExtensions
    {

        public static IServiceCollection RegisterMyServices(this IServiceCollection services, IConfiguration config)
        {
            try
            {
                //set up request/response logging
                var loggingHandler = GetHttpLoggingHandler(config);

                var baseUrl = config["IdentityFrameworkSettings:BaseUrl"];
                Uri endPoint = new Uri(baseUrl); // this is the endpoint HttpClient will hit
                HttpClient httpClient = new HttpClient(loggingHandler)
                {
                    BaseAddress = endPoint,
                };

                ServicePointManager.FindServicePoint(endPoint).ConnectionLeaseTimeout = 60000; // sixty seconds

                // Add all other services here.
                services.AddSingleton(httpClient);
                services.AddSingleton<IEmailService, EmailService>();


                return services;
            }
            catch (Exception)
            {

                throw;
            }
            
        }

        private static HttpLoggingHandler GetHttpLoggingHandler(IConfiguration config)
        {
            var loggingHandler = new HttpLoggingHandler
            (
                new HttpClientHandler(),
                HttpLoggingHandler.RequestAction,
                HttpLoggingHandler.ResponseAction
            );

            if (config["IdentityFrameworkSettings:KeepLogs"] != "0")
            {
                //if the backup directory is null, then use the current directory
                if (String.IsNullOrEmpty(config["IdentityFrameworkSettings:BackupDirectory"]))
                {
                    config["IdentityFrameworkSettings:BackupDirectory"] = AppDomain.CurrentDomain.BaseDirectory;
                }

                //set up logging for all request
                HttpLoggingHandler._BackupDirectory = config["IdentityFrameworkSettings:BackupDirectory"];
                HttpLoggingHandler._RequestLogFile = config["IdentityFrameworkSettings:RequestBackupFileName"];
                HttpLoggingHandler._ResponseLogFile = config["IdentityFrameworkSettings:ResponseBackupFileName"];
                HttpLoggingHandler._KeepLogs = Convert.ToInt32(config["IdentityFrameworkSettings:KeepLogs"]);
            }

            return loggingHandler;
        }
    }
}
