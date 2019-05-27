using Instantly.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace Instantly.Configuration
{
    public class AccountConfiguration : EntityTypeConfiguration<Account>
    {
        public AccountConfiguration()
        {
            ToTable("Accounts");

            Property(i => i.UserId).HasColumnName("UserId").IsRequired();
            Property(i => i.Login).HasColumnName("Login").IsRequired();
            Property(i => i.Password).HasColumnName("Password").IsRequired();

            Property(i => i.CountSendMessage).HasColumnName("CountSendMessage");
            Property(i => i.IsActive).HasColumnName("IsActive");
            Property(i => i.TextMessage).HasColumnName("TextMessage");
        }
    }
}