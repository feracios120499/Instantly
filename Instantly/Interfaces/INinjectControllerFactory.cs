﻿using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Instantly.Interfaces
{
    public interface INinjectControllerFactory
    {
        IKernel NinjectKernel { get; }
    }
}