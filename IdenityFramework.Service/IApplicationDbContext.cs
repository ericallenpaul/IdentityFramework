using IdentityFramework.Models;
using Microsoft.EntityFrameworkCore;

namespace IdentityFramework.Service
{
    public interface IApplicationDbContext
    {
        DbSet<CredentialType> CredentialTypes { get; set; }
        DbSet<Credential> Credentials { get; set; }
        DbSet<Role> Roles { get; set; }
        DbSet<UserRole> UserRoles { get; set; }
        DbSet<Permission> Permissions { get; set; }
        DbSet<RolePermission> RolePermissions { get; set; }
    }
}