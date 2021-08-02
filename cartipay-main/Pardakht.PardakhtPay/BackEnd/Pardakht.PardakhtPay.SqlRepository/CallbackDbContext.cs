//using Microsoft.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore.SqlServer.Infrastructure.Internal;
//using System;
//using System.Collections.Generic;
//using System.Text;
//using Pardakht.PardakhtPay.Shared.Models.Entities;

//namespace Pardakht.PardakhtPay.SqlRepository
//{
//    public class CallbackDbContext : DbContext
//    {
//        public CallbackDbContext(DbContextOptions<CallbackDbContext> options) : base(options)
//        {
//        }

//        public CallbackDbContext(string connectionString) : base(GetOptions(connectionString)) { }

//        private static DbContextOptions GetOptions(string connectionString)
//        {
//            var dbOptionsBuilder = new DbContextOptionsBuilder<CallbackDbContext>();
//            dbOptionsBuilder.UseSqlServer(connectionString);

//            return dbOptionsBuilder.Options;
//        }

//        public DbSet<TenantApi> TenantsApis { get; set; }
//    }
//}
