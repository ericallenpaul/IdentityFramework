# IdentityFramework
Put all of the pieces in place for the .Net Core Identity Framework and separate DBContext



Install .Net Core SDK and .Net Core Hosting Package
Download and Install Syncfusion ES2/MVC

Create the new projects:

Create Web Api - ProjectName.Api, .Net Core, Not RazorPages, Add Authentication for individual accounts
Create Command Line - ProjectName.Cli, .Net Core
Create The models Library - ProjectName.Models, .Net Standard 2.0
Create The Service Layer - ProjectName.Service, .Net Standard 2.0
Use Identity Framework
Break IF into seperate class
Hold the Database Context in a seperate class

Create the Web site - ProjectName.Web, .Net Core MVC, add Individual Authentication

Add Nuget Packages:

.Api
	Microsoft.Rest.ClientRunTime
	NLog.Web.AspNetCore
	Swashbuckle.AspNetCore
	Swashbuckle.AspNetCore.SwaggerUi
	Swashbuckle.AspNetCore.SwaggerGen
	Swashbuckle.AspNetCore.Annotations
	
.Cli
	CommandLineParser
	Newtonsoft.json
	Nlog
	Microsoft.Extensions.DependencyInjection
	StructureMap.Microsoft.DependencyInjection (https://andrewlock.net/using-dependency-injection-in-a-net-core-console-application/)

.Models
	Microsoft.AspNetCore.Identity.EntityFrameworkCore
	Microsoft.AspNetCore.Owin

.Service
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


	.Net standard compatibility
Add <AutoGenerateBindingRedirects>true</AutoGenerateBindingRedirects> to the initial property group
	
	
Project Configuration



Copy .Web ApplicationDbContext to service (change the namespace)

Copy .Web Migrations folder to services

Change the Namespace on the three .cs files and remove the using statement "using IdentityFramework.Web.Data;" (i.e. IdentityFramework.Web.Data.Migrations to IdentityFramework.Service

Delete the data folder from .Web

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
