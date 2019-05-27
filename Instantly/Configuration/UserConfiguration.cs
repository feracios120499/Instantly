using Instantly.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace Instantly.Configuration
{
    public class UserConfiguration : EntityTypeConfiguration<User>
    {
        public UserConfiguration()
        {
            ToTable("Users");

            Property(i => i.Id).HasColumnName("Id").IsRequired();
            Property(i => i.Password).HasColumnName("Password").IsRequired();
            Property(i => i.Email).HasColumnName("Email").IsRequired();
            Property(i => i.IsFree).HasColumnName("IsFree");
            Property(i => i.DateEndSub).HasColumnName("DateEndSub");

            HasMany(p => p.Accounts).WithRequired(p => p.User).HasForeignKey(p => p.UserId);
        }
    }
}