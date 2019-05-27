using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Instantly.Interfaces
{
    public interface IDatabaseFactory<TDbContext> : IDisposable where TDbContext : class
    {
        TDbContext Get();
    }
}