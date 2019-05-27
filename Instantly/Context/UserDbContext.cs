using Instantly.Configuration;
using Instantly.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Instantly.Context
{
    public class UserDbContext : DbContext
    {
        public UserDbContext(string connectiongString) : base(connectiongString)
        {
            Configuration.LazyLoadingEnabled = false;
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Account> Accounts { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new UserConfiguration());
            modelBuilder.Configurations.Add(new AccountConfiguration());
        }
    }
}