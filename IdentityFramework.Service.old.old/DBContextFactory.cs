using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace IdentityFramework.Service
{
    public class DbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<ApplicationDbContext>();
            builder.UseSqlServer(
                @"Server=(localdb)\MSSQLLocalDB;Database=IdentityFrameWork;AttachDBFilename=IdentityFramework.mdf;Trusted_Connection=True;MultipleActiveResultSets=true;");

            var returnValue = new ApplicationDbContext(builder.Options);

            return returnValue;
        }
    }
}
