using Buk.Gaming.Models;
using Buk.Gaming.Providers;
using Buk.Gaming.Repositories;
using Buk.Gaming.Services;
using Buk.Gaming.Web.Classes;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Buk.Gaming.Web.Services
{
    public abstract class BaseService
    {
        protected readonly IMemoryCache Cache;

        protected readonly ISessionProvider Session;

        protected BaseService(IMemoryCache cache, ISessionProvider session)
        {
            Cache = cache;
            Session = session;
        }
    }
}
