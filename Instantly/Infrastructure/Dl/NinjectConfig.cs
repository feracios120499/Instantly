using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Instantly.Infrastructure.Dl
{
    public static class NinjectConfig
    {
        private static IKernel _kernel;

        public static IKernel CreateKernel()
        {
            if (_kernel == null)
            {
                _kernel = new StandardKernel();
                _kernel.Load(new UserModule());
            }
            return _kernel;
        }
    }
}