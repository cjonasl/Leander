﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace WebApplication2
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }

        protected void Application_BeginRequest()
        {
            CarlJonasLeander.ApplicationBeginRequest(HttpContext.Current.Response, this.Context);
        }

        protected void Application_EndRequest()
        {
            CarlJonasLeander.ApplicationEndRequest(HttpContext.Current.Request, this.Context);
        }
    }
}
