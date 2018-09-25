using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace IdenityFramework.Service
{
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
}
