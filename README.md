# IdentityFramework
Put all of the pieces in place for the .Net Core Identity Framework and separate DBContext  
Break IF into seperate class  
Hold the Database Context in a seperate class  


Make sure your environment variables are set

Install .Net Core SDK and .Net Core Hosting Package  
Download and Install Syncfusion ES2/MVC  

### Create the new projects:

**1. Create an empty Solution** - [ProjectName].sln *(Listed under "other project types")*  
**2. Create Web Api** - [ProjectName].Api (.Net Core, Not RazorPages, Add Authentication for individual accounts)  
**3. Create Command Line** - [ProjectName].Cli, (.Net Core)  
**4. Create The models Library** - [ProjectName].Models (.Net Core)  
**5. Create The Service Layer**- [ProjectName].Service (.Net Core)  
**6. Create the Web site** - ProjectName.Web (.Net Core MVC, add Individual Authentication)  

### Add Nuget Packages:

###### .Api  
    Install-Package Microsoft.Rest.ClientRunTime
    Install-Package NLog.Web.AspNetCore
    Install-Package Swashbuckle.AspNetCore
    Install-Package Swashbuckle.AspNetCore.SwaggerUi
    Install-Package Swashbuckle.AspNetCore.SwaggerGen
    Install-Package Swashbuckle.AspNetCore.Annotations
    Install-Package Microsoft.AspNetCore.Owin
    Install-Package Microsoft.AspNetCore.Identity.EntityFrameworkCore 
    Install-Package Microsoft.AspNetCore.WebUtilities

	
###### .Cli
    Install-Package CommandLineParser
    Install-Package Newtonsoft.json
    Install-Package Nlog
    Install-Package Microsoft.Extensions.DependencyInjection
    Install-Package StructureMap.Microsoft.DependencyInjection (https://andrewlock.net/using-dependency-injection-in-a-net-core-console-application/)

###### .Models
    Nothing to do here

###### .Service
    Install-Package Microsoft.EntityFrameworkCore
	Install-Package Microsoft.EntityFrameworkCore.Design
	Install-Package Microsoft.EntityFrameworkCore.Tools
	Install-Package Microsoft.EntityFrameworkCore.SqlServer
	Install-Package Microsoft.AspNetCore.Identity.EntityFrameworkCore
	Install-Package Microsoft.AspNetCore.Owin
	Install-Package Newtonsoft.json
	Install-Package Nlog
	Install-Package System.Linq.Dynamic.Core
	Install-Package Automapper

### Service Configuration

Copy .Web ApplicationDbContext to service (change the namespace)

Copy .Web Migrations folder to services

Change the Namespace on the three .cs files (.Service.Migrations) and remove the using statement "`using IdentityFramework.Web.Data;`" (i.e. `IdentityFramework.Web.Data.Migrations` to `IdentityFramework.Service`)

Delete the data folder from .Web

Add a reference to the .Service project

Fix startup.cs. The startup file has:

	services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));
            services.AddDefaultIdentity<IdentityUser>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

since we moved `ApplicationDbContext` to the service project we need to add a using statement so it knows where to find it.  
Add

    using IdentityFramework.Service; 

to the startup.cs file.  

Now we're ready to scaffold out all of the pages. These include pages for all of the IdentityFramework supported functionality. Things like 2 factor authentication (2FA), Change password, User registration, etc.  
Scaffold identity into the web project by right-clicking on the project and selecting Add > New Scaffolded Item.  
From the left pane of the Add Scaffold dialog, select Identity then click ADD. In the ADD Identity dialog, select "override all files".  
Select the Data context class from the .Service project (ApplicationDbContext).  
Click the ADD button.  
This will create all of the pages need to allow a user to register and login.  

### API Configuration
Now it's time to work on our API project. The API Project will use Swagger which will both provide documentation and provide an easy way to test our code.

##### Configure Swagger 
Swagger will automatically generate both a testing platform and include documentation. It pulls the documentation for the standard XML documentation comments, so we need to turn that on in both the API and model projects.  
Right-click on the API project and choose properties. Then click the build tab, scroll down, and check the "XML documentation file" box. Repeat this for the models project. Make sure you note the name of the files, we'll need these for the next step.  
Create a new class called SwaggerHelper add the following using statements:

    using Microsoft.AspNetCore.Builder;
    using Swashbuckle.AspNetCore.Swagger;
    using Swashbuckle.AspNetCore.SwaggerUI;
    using Swashbuckle.AspNetCore.SwaggerGen;

and paste in the following code

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

            //include the XML documentation
            swaggerGenOptions.DescribeAllEnumsAsStrings();
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "IdenityFramework.API.xml");
            swaggerGenOptions.IncludeXmlComments(filePath);
            filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "IdentityFramework.Models.xml");
            swaggerGenOptions.IncludeXmlComments(filePath);

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
Set the startup to match the specified url "api-docs". Richt-click the project and select properties. On the build tab set the field next to "Launch Browser" to "api-docs".

Now we need to add Swagger to the startup.cs file. In the ConfigureServiceMethod add:

services.AddSwaggerGen(SwaggerHelper.ConfigureSwaggerGen);

then in the configure method add

    app.UseSwagger(SwaggerHelper.ConfigureSwagger);
    app.UseSwaggerUI(SwaggerHelper.ConfigureSwaggerUI);

At this point swagger is configured and ready to use. You can add xml documentation and the swagger attributes to the existing "ValuesController". Add the using statement:

    using Swashbuckle.AspNetCore.Annotations;

and then add some attributes the first Get method:

    /// <summary>
    /// Gets some values.
    /// </summary>
    /// <returns></returns>
    [ProducesResponseType(typeof(string[]), 200)]
    [ProducesResponseType(typeof(NotFoundObjectResult), 400)]
    [ProducesResponseType(500)]
    [SwaggerOperation(OperationId = "GetSomeValues")]
    [HttpGet]
    [Route("api/v1/GetSomeValues", Name = "GetSomeValues")]
    public ActionResult<IEnumerable<string>> GetSomeValues()
    {
        
        return new string[] { "value1", "value2" };
    }   

Add similar attributes to the rest of the methods.
You will need to change the method name of the second "Get" statement, because swagger won't work if you have multiple methods with the same name which are the same http action (even if they take different parameters). You can just change it to "GetById" and everything should work.
Now you should be able to run the API and see the swagger interface:

![swaggerImage](https://github.com/ericallenpaul/IdentityFramework/blob/master/swagger.png?raw=true)


##### Add Settings
Add Appsettings.Production.json

##### Configure AutoMapper

##### Configure Identity  
Since we already installed the correct libraries all we need to do now is configure IdentityFramework in the API. Start by changing the MVC Config.  
Add the following using statements:

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc.Authorization;

then change:

    services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
    
to:

    services.AddMvc(config =>
    {
        var policy = new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .Build();
        config.Filters.Add(new AuthorizeFilter(policy));
    }).SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

Also add a using statement

	using IdentityFramework.Service;

and then add

     services.AddDbContext<ApplicationDbContext>(options =>
          options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));
                    
     services.AddDefaultIdentity<IdentityUser>()
          .AddEntityFrameworkStores<ApplicationDbContext>();
                
Also add

     app.UseCookiePolicy();
     app.UseAuthentication();
             
Generate Terms Of service - https://formswift.com/terms-of-service, https://termsofservicegenerator.net/, https://termsfeed.com/terms-service/generator/ ($$$)




http://sikorsky.pro/en/blog/aspnet-core-custom-user-manager
https://github.com/DmitrySikorsky/AspNetCoreCustomUserManager/tree/master/AspNetCoreCustomUserManager/Data






.Cli
 Add some configuration files to support dependencey injection

 
 
 Enumeration Classes
 https://docs.microsoft.com/en-us/dotnet/standard/microservices-architecture/microservice-ddd-cqrs-patterns/enumeration-classes-over-enum-types
 https://lostechies.com/jimmybogard/2008/08/12/enumeration-classes/
 https://eliot-jones.com/2015/3/entity-framework-enum
