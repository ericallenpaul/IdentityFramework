using System.Security.Claims;
using System.Threading.Tasks;
using IdentityFramework.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace IdentityFramework.Service
{

    public class ApplicationDbContext : DbContext, IApplicationDbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<CredentialType> CredentialTypes { get; set; }
        public DbSet<Credential> Credentials { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        private override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.RemovePluralizingTableNameConvention();

            modelBuilder.Entity<User>(etb =>
                {
                    etb.HasKey(e => e.Id);
                    etb.Property(e => e.Id).ValueGeneratedOnAdd();
                    etb.Property(e => e.Name).IsRequired().HasMaxLength(64);
                    etb.ToTable("Users");
                }
            );

            modelBuilder.Entity<CredentialType>(etb =>
                {
                    etb.HasKey(e => e.Id);
                    etb.Property(e => e.Id).ValueGeneratedOnAdd();
                    etb.Property(e => e.Code).IsRequired().HasMaxLength(32);
                    etb.Property(e => e.Name).IsRequired().HasMaxLength(64);
                    etb.ToTable("CredentialTypes");
                }
            );

            modelBuilder.Entity<Credential>(etb =>
                {
                    etb.HasKey(e => e.Id);
                    etb.Property(e => e.Id).ValueGeneratedOnAdd();
                    etb.Property(e => e.Identifier).IsRequired().HasMaxLength(64);
                    etb.Property(e => e.Secret).HasMaxLength(1024);
                    etb.ToTable("Credentials");
                }
            );

            modelBuilder.Entity<Role>(etb =>
                {
                    etb.HasKey(e => e.Id);
                    etb.Property(e => e.Id).ValueGeneratedOnAdd();
                    etb.Property(e => e.Code).IsRequired().HasMaxLength(32);
                    etb.Property(e => e.Name).IsRequired().HasMaxLength(64);
                    etb.ToTable("Roles");
                }
            );

            modelBuilder.Entity<UserRole>(etb =>
                {
                    etb.HasKey(e => new { e.UserId, e.RoleId });
                    etb.ToTable("UserRoles");
                }
            );

            modelBuilder.Entity<Permission>(etb =>
                {
                    etb.HasKey(e => e.Id);
                    etb.Property(e => e.Id).ValueGeneratedOnAdd();
                    etb.Property(e => e.Code).IsRequired().HasMaxLength(32);
                    etb.Property(e => e.Name).IsRequired().HasMaxLength(64);
                    etb.ToTable("Permissions");
                }
            );

            modelBuilder.Entity<RolePermission>(etb =>
                {
                    etb.HasKey(e => new { e.RoleId, e.PermissionId });
                    etb.ToTable("RolePermissions");
                }
            );

            //create some unique indexes
            //modelBuilder.Entity<AllowedIP>().HasIndex(ip => new { ip.IpAddress }).IsUnique(true);

            modelBuilder.Seed();
        }
    }
}
