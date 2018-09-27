using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IdentityFramework.Service
{

    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.RemovePluralizingTableNameConvention();

           

            //create some unique indexes
            //modelBuilder.Entity<AllowedIP>().HasIndex(ip => new { ip.IpAddress }).IsUnique(true);

            modelBuilder.Seed();
        }
    }
}
