# IdentityFramework -- THIS IS A WORK IN PROGRESS, It's not Finishsed
Put all of the pieces in place for the .Net Core Identity Framework and separate DBContext  
Break IF into seperate class  
Hold the Database Context in a seperate class  


JWT
https://jwt.io/introduction/

Token Claim Names
https://www.iana.org/assignments/jwt/jwt.xhtml



Refresh Tokens

PlatformServices is obsolete
https://developers.de/blogs/holger_vetter/archive/2017/06/30/swagger-includexmlcomments-platformservices-obsolete-replacement.aspx

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
    Install-Package Microsoft.Extensions.PlatformAbstractions  
    Install-Package NLog.Extensions.Logging
    Install-Package Microsoft.IdentityModel.Tokens
    
    Microsoft.AspNetCore.Authentication
    Microsoft.AspNetCore.Session
    Microsoft.AspNetCore.HttpsPolicy
    Microsoft.AspNetCore.CookiePolicy
    Microsoft.AspNetCore.StaticFiles
    Microsoft.AspNetCore 
    
    
    StructureMap.Microsoft.DependencyInjection

	
###### .Cli
    Install-Package CommandLineParser
    Install-Package Newtonsoft.json
    Install-Package Nlog
    Install-Package Microsoft.Extensions.DependencyInjection
    Install-Package StructureMap.Microsoft.DependencyInjection (https://andrewlock.net/using-dependency-injection-in-a-net-core-console-application/)

###### .Models
    Install-Package Microsoft.AspNetCore.Identity.EntityFrameworkCore 

###### .Service
    Install-Package Microsoft.EntityFrameworkCore
	Install-Package Microsoft.EntityFrameworkCore.Design
	Install-Package Microsoft.EntityFrameworkCore.Tools
	Install-Package Microsoft.EntityFrameworkCore.SqlServer
	Install-Package Microsoft.AspNetCore.Identity.EntityFrameworkCore
	Install-Package Microsoft.AspNetCore.Owin
	Install-Package Newtonsoft.json
	Nlog
	Install-Package System.Linq.Dynamic.Core
	Install-Package Automapper
	Install-Package Microsoft.AspNetCore.Mvc

##### .Web
    ??? Install-Package Microsoft.Rest.ClientRunTime
    Install-Package NLog.Web.AspNetCore

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
            
            swaggerGenOptions.AddSecurityDefinition("Bearer", new ApiKeyScheme
            {
                Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                Name = "Authorization",
                In = "header",
                Type = "apiKey"
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
        public string SmtpServer { get; set; }
        public string FromAddress { get; set; }
        public string FromName { get; set; }
        public string ConfirmEmailUrl { get; set; }
        public bool LockoutOnFailure { get; set; }
        public string BaseUrl { get; set; }
        public string ApiVersion { get; set; }
        public string BackupDirectory { get; set; }
        public int KeepLogs { get; set; } = 60;
        public string RequestBackupFileName { get; set; } = "_Request";
        public string ResponseBackupFileName { get; set; } = "_Response";
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
      "ConnectionStrings": {
        "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=IdentityFrameworkDb;Trusted_Connection=True;MultipleActiveResultSets=true"
      },
      "IdentityFrameworkSettings": {
        "SmtpServer": "localhost",
        "FromAddress": "if@IdentityFraneWork.com",
        "FromName": "Identity Framework",
        "ConfirmEmailUrl": "/Account/ConfirmEmail",
        "LockoutOnFailure": "true",
        "BaseUrl": "http:\\\\IdentityFramework.local",
        "ApiVersion":  "v1" 
      },
      "IdentityFramework_JWT": {
        "SecretKey": "79dcc55f-1992-4182-b285-b2d0196e9e55",
        "Issuer": "http://identityframework.com",
        "Audience": "Identity Framework"
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

add nlog config


		<?xml version="1.0" encoding="utf-8" ?>
		<nlog xmlns="http://www.nlog-project.org/schemas/NLog.xsd"
			  xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
			  xsi:schemaLocation="http://www.nlog-project.org/schemas/NLog.xsd NLog.xsd"
			  autoReload="true"
			  throwExceptions="false"
			  internalLogLevel="Off" internalLogFile="c:\temp\nlog-internal.log">

		  <targets>
			<!-- log to the console -->
			<target name="console" xsi:type="ColoredConsole" layout="${date:format=HH\:mm\:ss}|${level}|${message}" />

			<!-- log to a file -->
			<target xsi:type="File"
					name="allfile"
					fileName="${basedir}\logs\IdentityFramework\nlog-all.log"
					archiveFileName="${basedir}\logs\IdentityFramework\nlog-all.{#}.txt"
					archiveEvery="Day"
					archiveNumbering="Rolling"
					maxArchiveFiles="7"
					layout="${longdate}|${machinename}|${event-properties:item=EventId.Id}|${uppercase:${level}}|${logger}|${message} ${exception}" />
			
			<!-- no logging -->
			<target xsi:type="Null" name="blackhole" />
		  </targets>

		  <rules>
			<!--All logs, including from Microsoft-->
			<logger name="*" minlevel="Trace" writeTo="console, allfile" />

			<!--Skip Microsoft logs and so log only own logs-->
			<logger name="Microsoft.*" minlevel="Trace" writeTo="blackhole" final="true" />
			<logger name="*" minlevel="Trace" writeTo="ownFile-web" />
		  </rules>
		</nlog>



set to content and copy always


See https://github.com/nlog/nlog/wiki/Configuration-file
  for information on customizing logging rules and outputs.

##### Configure Identity  
Since we already installed the correct libraries all we need to do now is configure IdentityFramework in the API. Start by changing the MVC Config.  

Add a class to models:

IdentityFramework_JWT

add a using statement:

    using Microsoft.IdentityModel.Tokens;

Then add the class and Interface

    public interface IIdentityFramework_JWT
    {
        string SecretKey { get; set; }
        string Issuer { get; set; }
        string Audience { get; set; }
        string Username { get; set; }
        SigningCredentials SigningCredentials { get; set; }
    }
    
    public class IdentityFramework_JWT
    {
        public string SecretKey { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public string Username { get; set; }

        public SigningCredentials SigningCredentials { get; set; }
    }

Then in the API Startup.cs file add wiring for the new class:

            services.Configure<IdentityFramework_JWT>(Options =>
                Configuration.GetSection("IdentityFramework_JWT").Bind(Options));

Add the settings in the .JSON file

      "IdentityFramework_JWT": {
        "SecretKey": "79dcc55f-1992-4182-b285-b2d0196e9e55",
        "Issuer": "http://identityframework.com",
        "Audience": "Identity Framework"
      }


add the configuration to Startup.cs

            //add a new auth policy
            //authorize with "var credentials = new TokenCredentials("<bearer token>");"
            services.AddAuthorization(auth =>
            {
                auth.AddPolicy("Bearer", new AuthorizationPolicyBuilder()
                    .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                    .RequireAuthenticatedUser().Build());
            });

            //add the JWT authentication
            services.AddAuthentication(options => { options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme; })
                .AddJwtBearer(cfg =>
                {
                    cfg.RequireHttpsMetadata = false;
                    cfg.SaveToken = true;

                    cfg.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidIssuer = Configuration["IdentityFramework_JWT:Issuer"],
                        ValidAudience = Configuration["IdentityFramework_JWT:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration["IdentityFramework_JWT:SecretKey"]))
                    };

                });
                
Add the using statements to the userservice:

using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

Add a private property to be injected

        private readonly IIdentityFramework_JWT _TokenOptions;

Add to the constructor params:

     IOptions<IdentityFramework_JWT> TokenOptions,

In the constructor add:

     this._TokenOptions = TokenOptions.Value;




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
                options.DefaultAuthenticateScheme = "Jwt";
                options.DefaultChallengeScheme = "Jwt";
            }).AddJwtBearer("Jwt", options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = false,
                    //ValidAudience = "the audience you want to validate",
                    ValidateIssuer = false,
                    //ValidIssuer = "the isser you want to validate",

                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("the secret that needs to be at least 16 characeters long for HmacSha256")),

                    ValidateLifetime = true, //validate the expiration and not before values in the token

                    ClockSkew = TimeSpan.FromMinutes(5) //5 minute tolerance for the expiration date
                };
            });
                
Also add

     app.UseCookiePolicy();
     app.UseAuthentication();
             
             

### Create Email Service
create class

create settings



#### Customize Identity  
Out of the box the identity Framework doesn't support what some might consider standard fields in the user table. It doesn't have First name or last name, for example. So most will want to add some additional columns to the user table. Fortunately this can be done by just extending the existing the existing framework. (You can also override it entirely and create a complete;y custom implementation.) We will aslo add some custom "claims" (permissions) since the only ones supported are UserId and Username.  
In order to do this we'll need to add 2 classes and then inject them into the configuration. First our custom user class will be added to the models project. Create a class called `ApplicationUser` add the following useing statement:

    using Microsoft.AspNetCore.Identity;

Then add this code for the class:

    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Address { get; set; }
        
        public string City { get; set; }
        
        public string State { get; set; }

        [Display(Name = "Postal Code")]
        public string PostalCode { get; set; }
    }

Next we need to add a class to services that will add first name and last name to our claims. Create a class called `AppClaimsPrincipalFactory`. Add the following using statements:

    using Microsoft.AspNetCore.Identity;
    using System.Security.Claims;
    
 Then add this code for the class:
 
     public class AppClaimsPrincipalFactory : UserClaimsPrincipalFactory<ApplicationUser, IdentityRole>
    {
        public AppClaimsPrincipalFactory(
            UserManager<ApplicationUser> userManager
            , RoleManager<IdentityRole> roleManager
            , IOptions<IdentityOptions> optionsAccessor)
            : base(userManager, roleManager, optionsAccessor)
        { }

        public async override Task<ClaimsPrincipal> CreateAsync(ApplicationUser user)
        {
            var principal = await base.CreateAsync(user);

            if (!string.IsNullOrWhiteSpace(user.FirstName))
            {
                ((ClaimsIdentity)principal.Identity).AddClaims(new[] {
                    new Claim(ClaimTypes.GivenName, user.FirstName)
                });
            }

            if (!string.IsNullOrWhiteSpace(user.LastName))
            {
                ((ClaimsIdentity)principal.Identity).AddClaims(new[] {
                    new Claim(ClaimTypes.Surname, user.LastName),
                });
            }

            return principal;
        }
    }

In the startup.cs class we need to add the following line:

            services.AddScoped<Microsoft.AspNetCore.Identity.IUserClaimsPrincipalFactory<ApplicationUser>, AppClaimsPrincipalFactory>();



             
### Configuring the user service             
             
 add ref to 
 using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;



JWT Offical Claims: https://tools.ietf.org/html/rfc7519#page-9
             
             
             
             
Generate Terms Of service - https://formswift.com/terms-of-service, https://termsofservicegenerator.net/, https://termsfeed.com/terms-service/generator/ ($$$)




http://sikorsky.pro/en/blog/aspnet-core-custom-user-manager
https://github.com/DmitrySikorsky/AspNetCoreCustomUserManager/tree/master/AspNetCoreCustomUserManager/Data






.Cli
 Add some configuration files to support dependencey injection

 
 
 Enumeration Classes
 https://docs.microsoft.com/en-us/dotnet/standard/microservices-architecture/microservice-ddd-cqrs-patterns/enumeration-classes-over-enum-types
 https://lostechies.com/jimmybogard/2008/08/12/enumeration-classes/
 https://eliot-jones.com/2015/3/entity-framework-enum

 
 #### Dependecey injection duplication
https://medium.com/tech-feed/asp-net-core-web-api-avoid-dependency-injection-duplication-6dacdeb36454

https://www.strathweb.com/2018/06/controllers-as-action-filters-in-asp-net-core-mvc/


Fix _LoginPartial
