using Instantly.Context;
using Instantly.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Instantly.Infrastructure.DbFactory
{
    public class UserDbFactory : Disposable, IDatabaseFactory<UserDbContext>
    {

        private UserDbContext _dataContext;
        /// <summary>
        /// User database context
        /// </summary>
        /// <returns></returns>
        public UserDbContext Get()
        {
            return _dataContext ??
                (_dataContext = new UserDbContext("DefaultConnection"));
        }

        /// <summary>
        /// Dispose Core
        /// </summary>
        protected override void DisposeCore()
        {
            _dataContext?.Dispose();
        }
    }
}