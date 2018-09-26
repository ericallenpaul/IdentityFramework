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

Create the Web site - ProjectName.Web, .Net Core MVC

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
	Nothing to do here

.Service
	Microsoft.EntityFrameworkCore
	Microsoft.EntityFrameworkCore.Design
	Microsoft.EntityFrameworkCore.Tools
	Microsoft.EntityFrameworkCore.SqlServer
	Microsoft.AspNet.Identity.EntityFramework
	Microsoft.AspNet.Identity.Owin
	Newtonsoft.json
	Nlog
	DnsClient
	System.Linq.Dynamic.Core



Project Configuration

.Service

Add a new class file named "ApplicationDbContext".
Add the following code:

public class ApplicationUser : IdentityUser
{
	public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager, string authenticationType)
	{
		// Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
		var userIdentity = await manager.CreateIdentityAsync(this, authenticationType);
		// Add custom user claims here
		return userIdentity;
	}
}

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
	public ApplicationDbContext() : base("DefaultConnection", throwIfV1Schema: false)
	{
	}

	public static ApplicationDbContext Create()
	{
		return new ApplicationDbContext();
	}
}



Add a model builder extensions class.
Put in the follwoing code:

public static class ModelBuilderExtensions
{
	public static void RemovePluralizingTableNameConvention(this ModelBuilder modelBuilder)
	{
		foreach (IMutableEntityType entity in modelBuilder.Model.GetEntityTypes())
		{
			entity.Relational().TableName = entity.DisplayName();
		}
	}

	public static void Seed(this ModelBuilder modelBuilder)
	{
		//seed the DB
		//modelBuilder.Entity<SOME_Custom_Object>().HasData(

		//    new SOME_Custom_Object { Prop1 = 1, Prop2 = "zoo" },
		//    new SOME_Custom_Object { Prop1 = 2, Prop2 = "zoo2" }
		//);

	}
}

Add a DBContext class














.Web

Scaffold identity into an empty project
From Solution Explorer, right-click on the project > Add > New Scaffolded Item.
From the left pane of the Add Scaffold dialog, select Identity > ADD.
In the ADD Identity dialog, select all options.
Select your existing layout page  ~/Pages/Shared/_Layout.cshtml 
Select the exsiting Data context class (ApplicationDbContext).
Select ADD.

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
