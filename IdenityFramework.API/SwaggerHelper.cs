
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerUI;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace IdentityFramework.API
{
    public class SwaggerHelper
    {
        public static void ConfigureSwaggerGen(SwaggerGenOptions swaggerGenOptions)
        {
            swaggerGenOptions.SwaggerDoc("v1", new Info
            {
                Title = "Identity Framework",
                Version = $"v1",
                Description = "An API for testing the Identity Framework"
            });

            swaggerGenOptions.AddSecurityDefinition("Bearer", new ApiKeyScheme
            {
                Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                Name = "Authorization",
                In = "header",
                Type = "apiKey"
            });

            //include the XML documentation
            //swaggerGenOptions.DescribeAllEnumsAsStrings();
            //string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "IdenityFramework.API.xml");
            //if (File.Exists(filePath))
            //{
            //    swaggerGenOptions.IncludeXmlComments(filePath);
            //}

            //filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "IdentityFramework.Models.xml");
            //if (File.Exists(filePath))
            //{
            //    swaggerGenOptions.IncludeXmlComments(filePath);
            //}

        }

        public static void ConfigureSwagger(SwaggerOptions swaggerOptions)
        {
            swaggerOptions.RouteTemplate = "api-docs/{documentName}/swagger.json";
        }

        public static void ConfigureSwaggerUI(SwaggerUIOptions swaggerUIOptions)
        {
            swaggerUIOptions.SwaggerEndpoint($"/api-docs/v1/swagger.json", $"v1 Docs");
            swaggerUIOptions.RoutePrefix = "api-docs";
        }
    }
}
