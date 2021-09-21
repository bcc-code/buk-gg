using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Buk.Gaming.Providers;
using Buk.Gaming.Repositories;
using Buk.Gaming.Models;
using Buk.Gaming.Toornament;
using Buk.Gaming.Toornament.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Buk.Gaming.Services;

namespace Buk.Gaming.Web.Controllers
{
    public abstract class BaseControllerBase : ControllerBase
    {
        protected readonly ISessionProvider Session;

        protected BaseControllerBase(ISessionProvider session)
        {
            Session = session;
        }
    }
}