# IdentityFramework
Put all of the pieces in place for the .Net Core Identity Framework and separate DBContext
Break IF into seperate class
Hold the Database Context in a seperate class


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

### Project Configuration



Copy .Web ApplicationDbContext to service (change the namespace)

Copy .Web Migrations folder to services

Change the Namespace on the three .cs files and remove the using statement "using IdentityFramework.Web.Data;" (i.e. IdentityFramework.Web.Data.Migrations to IdentityFramework.Service

Delete the data folder from .Web

Add a refrence to the .Service project

Fix startup -- add using statement

Scaffold identity into the web project, right-click on the project > Add > New Scaffolded Item. 
From the left pane of the Add Scaffold dialog, select Identity > ADD. In the ADD Identity dialog, select all options. 
Select your existing layout page ~/Pages/Shared/_Layout.cshtml Select the exsiting Data context class (ApplicationDbContext). Select ADD.

Add Appsettings.Production.json

Configure Identity



Generate Terms Of service - https://formswift.com/terms-of-service, https://termsofservicegenerator.net/, https://termsfeed.com/terms-service/generator/ ($$$)




http://sikorsky.pro/en/blog/aspnet-core-custom-user-manager
https://github.com/DmitrySikorsky/AspNetCoreCustomUserManager/tree/master/AspNetCoreCustomUserManager/Data






.Cli
 Add some configuration files to support dependencey injection

 
 
 Enumeration Classes
 https://docs.microsoft.com/en-us/dotnet/standard/microservices-architecture/microservice-ddd-cqrs-patterns/enumeration-classes-over-enum-types
 https://lostechies.com/jimmybogard/2008/08/12/enumeration-classes/
 https://eliot-jones.com/2015/3/entity-framework-enum
