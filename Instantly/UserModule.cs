using Instantly.Context;
using Instantly.Infrastructure.DbFactory;
using Instantly.Interfaces;
using Ninject.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Ninject.Web.Common;
using Ninject.Web.WebApi.Filter;
using System.Web.ModelBinding;
using Instantly.Infrastructure.Repository;
using System.Diagnostics;

namespace Instantly
{
    public class UserModule : NinjectModule
    {
        public override void Load()
        {

            this.Bind<IDatabaseFactory<UserDbContext>>().To<UserDbFactory>().InRequestScope();
            this.Bind<IAccountRepository>().To<AccountRepository>().InRequestScope();
        }
    }
}