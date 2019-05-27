using Instantly.Interfaces;
using Ninject;
using Ninject.Web.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;

namespace Instantly.Infrastructure.Dl
{
    public class NinjectControllerFactory : DefaultControllerFactory, INinjectControllerFactory
    {
        //делаем public для того чтобы при регистрации areas сделать дополнительные bind-инги
        public NinjectControllerFactory(IKernel ninjectKernel)
        {
            NinjectKernel = ninjectKernel;
            var resolver = new NinjectDependencyResolver(NinjectKernel);

            GlobalConfiguration.Configuration.DependencyResolver = resolver;
        }

        public IKernel NinjectKernel { get; private set; }

        protected override IController GetControllerInstance(RequestContext requestContext, Type controllerType)
        {
            return controllerType == null
                ? null
                : (IController)NinjectKernel.Get(controllerType);
        }
    }
}