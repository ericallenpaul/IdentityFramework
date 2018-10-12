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
    
    NLog.Extensions.Logging
    NLog.config
    
    StructureMap.Microsoft.DependencyInjection

	
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
Now it's time to work on our API project. The API Project will use Swagger which will provide both documentation and an easy way to test our code.

##### Configure Swagger 
Swagger is pretty much the best thing since sliced bread. It allows us to test and document our API all at the same time. It pulls the documentation from standard XML documentation comments, so we need to turn on XML documentation in both the API and model projects.  
Right-click on the API project and choose properties. Then click the build tab, scroll down, and check the "XML documentation file" box. Repeat this for the models project. Make sure you note the name of the files, we'll need these for the next step.  

![xmlDocs](https://github.com/ericallenpaul/IdentityFramework/blob/master/XmlDocs.png?raw=true)

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

![vsProperties](https://github.com/ericallenpaul/IdentityFramework/blob/master/SetOptions.png?raw=true)

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
Now it's time to add settings to our API. Settings in .Net core are strongly typed and all driven by JSON files.
The first thing we need to do is add a settings class. Add a class called IdentityFrameworkSettings. 
Populate this class with properties that match the settings in the JSON file. For now will just add one property "ConnectionString". (In a more simple configuration ConnectionString would get it's own section but since we'll be injecting this string into our service we'll make it a setting.)

    public class IdentityFrameworkSettings
    {
        public string ConnectionString { get; set; }
    }

Now we need to add the corresponding property to our JSON file, specifically the appsettings.Development.json file. This is what it will look like after our settings section is added:

    {
      "Logging": {
        "LogLevel": {
          "Default": "Debug",
          "System": "Information",
          "Microsoft": "Information"
        }
      },
      "IdentityFrameworkSettings": {
        "ConnectionString": "Server=(localdb)\\MSSQLLocalDB;Database=IdentityFrameworkDb;Trusted_Connection=True;MultipleActiveResultSets=true"
      }
    }

We used the same connection string that we used in our service "IDesignTimeDbContextFactory".
Now we need to add a settings file for production. Right-click and select "New Item..." and then Web and look for "App Settings File". Name it appsettings.Production.json and click save.

![appsettings](https://github.com/ericallenpaul/IdentityFramework/blob/master/appsettings.png?raw=true)

Copy and paste the development settings file and change values as needed. There really is no limit to the number of settings file you can have. You can have one for each environment so you could have appsettings.Testing.Json, appsettings.Staging.json, etc.
Next we need to add a bit of code to get the project to honor our environment variables. In `program.cs` I add a static string to reference the environment variable and an IconFigurationRoot variable:

    private static IConfigurationRoot _configuration;
	private static string env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

Then I add the following configuration code in the main method:

            _configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env}.json", optional: true)
                .Build();

Now all I need to do is make sure I have the right environment variables set. Environment variables can be set a number of different ways but I think the easiest is to just use powershell. Open an administrative powershell window and enter the following commands:

    [Environment]::SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development", "User")
    [Environment]::SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development", "Machine")

The last step is to wire up the settings in the `startup.cs` file. In the configure services method I added the following code:

            services.Configure<IdentityFrameworkSettings>(Options =>
                Configuration.GetSection("IdentityFrameworkSettings").Bind(Options));

Now we have a class that will be populated with all of our settings. Any Controller with a constructor using "IdentityFrameworkSettings" will automatically be wired up through dependency injection.

##### Configure AutoMapper
Now we need to configure Automapper. Automapper is a great little tool to map those partial classes (DTOs) back to a parent/target class. 

#### Configure Nlog
NLog.Extensions.Logging
NLog.config

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
          
          
          
          
          
          
          
                      services.AddAuthentication(options =>
                {
                    //options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    //options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                });
                
Also add

     app.UseCookiePolicy();
     app.UseAuthentication();
             
             
             
             
             
### Configuring the user service             
             
 add ref to 
 using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;


             
             
             
             
Generate Terms Of service - https://formswift.com/terms-of-service, https://termsofservicegenerator.net/, https://termsfeed.com/terms-service/generator/ ($$$)




http://sikorsky.pro/en/blog/aspnet-core-custom-user-manager
https://github.com/DmitrySikorsky/AspNetCoreCustomUserManager/tree/master/AspNetCoreCustomUserManager/Data






.Cli
 Add some configuration files to support dependencey injection

 
 
 Enumeration Classes
 https://docs.microsoft.com/en-us/dotnet/standard/microservices-architecture/microservice-ddd-cqrs-patterns/enumeration-classes-over-enum-types
 https://lostechies.com/jimmybogard/2008/08/12/enumeration-classes/
 https://eliot-jones.com/2015/3/entity-framework-enum
