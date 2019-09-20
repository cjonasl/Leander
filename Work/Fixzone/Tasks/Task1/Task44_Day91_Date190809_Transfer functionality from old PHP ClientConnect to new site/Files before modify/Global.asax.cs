using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using ClientConnect.App_Start;
using ClientConnect.Configuration;
using ClientConnect.Infrastructure;
using ClientConnect.Logging;
using ClientConnect.Process;
using ClientConnect.Services;
using ClientConnect.UserAccount;
using FluentValidation.Mvc;
using ClientConnect;
using System.Collections.Generic;


namespace ClientConnectApplication
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        /// <summary>
        /// Logger
        /// </summary>
        //public static readonly ILog Logger = LogManager.GetLogger(typeof(MvcApplication));


        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new CheckStoreFilter());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");


            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );
            routes.MapRoute(
                "Details", // Route name
                "{Customer}/{Details}/{id}", // URL with parameters
                new { controller = "Customer", action = "Details" } // Parameter defaults
            );

            routes.MapRoute(
                "SignIn", // Route name
                string.Format("Process/Go/{0}", (int)PredefinedProcess.SignIn), // URL with parameters
                new { controller = "Process", action = "Go", id = PredefinedProcess.SignIn } // Parameter defaults
            );
            routes.MapRoute(
               "CallCenterSignIn", // Route name
             "CallCenter/SignIn"
           );
            routes.MapRoute(
                "SignOut", // Route name
                "Account/SignOut", // URL with parameters
                new { controller = "Account", action = "SignOut" } // Parameter defaults
            );

            routes.MapRoute(
                "ChangePassword", // Route name
                string.Format("Process/Go/{0}", (int)PredefinedProcess.ExpiredPassword), // URL with parameters
                new { controller = "Process", action = "Go", id = PredefinedProcess.ExpiredPassword } // Parameter defaults
            );

            routes.MapRoute(
                "ForgottenPassword", // Route name
                string.Format("Process/Go/{0}", (int)PredefinedProcess.UserForgottenPassword), // URL with parameters
                new { controller = "Process", action = "Go", id = PredefinedProcess.UserForgottenPassword } // Parameter defaults
            );

            routes.MapRoute(
                "AccessDenied", // Route name
                "Account/AccessDenied/{id}",
                new { controller = "Process", action = "Go", id = UrlParameter.Optional } // Parameter defaults
            );
        }

        protected void Application_Start()
        {
            try
            {
                IocRetailerConnectModule.Register();

                BundleConfig.RegisterBundles(BundleTable.Bundles);

                // Initilize logger

                log4net.Config.XmlConfigurator.Configure();
                AreaRegistration.RegisterAllAreas();

                RegisterGlobalFilters(GlobalFilters.Filters);
                RegisterRoutes(RouteTable.Routes);
                FluentValidationModelValidatorProvider.Configure();
               Log.File.InfoFormat( Server.MapPath("~/license.key"));
                Stimulsoft.Base.StiLicense.LoadFromFile(Server.MapPath("~/license.key"));
            }
            catch(Exception ex)
            {
                var _error = new ErrorsService();
                Log.File.ErrorFormat(_error.Msg.GenerateLogMsg(string.Format("Unhandled error on page. Error:{0}", ex.Message)));
            }
        }

        protected void Session_start()
        {
        }

        protected void Application_Error()
        {
            Exception curException = Server.GetLastError();
            var httpException = curException as HttpException;
            if (httpException == null)
            {
                Exception innerException = curException.InnerException;
                httpException = innerException as HttpException;
            }
            Server.ClearError();
            var _error = new ErrorsService();
            if (httpException != null)
            {
                int httpCode = httpException.GetHttpCode();
                
                _error.ErrorMessage = httpException.Message;
                
                // here we can add custom pages for different HTTP codes
                switch (httpCode)
                {
                    case 400:
                        {
                            Log.File.InfoFormat(_error.Msg.GenerateLogMsg(string.Format("Error {0}: {1}. ", httpCode, httpException.Message)));
                            Log.File.ErrorFormat(_error.Msg.GenerateLogMsg(string.Format("Error {0}: {1}. ", httpCode, httpException.Message)));
                            Response.Redirect("/Error/http400");
                            return;
                        }

                    case 404:
                        {
                            Log.File.InfoFormat(_error.Msg.GenerateLogMsg(string.Format("Error {0}: {1}. ", httpCode, httpException.Message)));
                            Log.File.ErrorFormat(_error.Msg.GenerateLogMsg(string.Format("Url {0} was not found", Request.Url)));
                            Response.Redirect("/Error/http404");
                            return;
                        }
                    case 500:
                        {
                            Log.File.InfoFormat(_error.Msg.GenerateLogMsg(string.Format("Error {0}: {1}. ", httpCode, httpException.Message)));
                            Log.File.ErrorFormat(_error.Msg.GenerateLogMsg(string.Format("Error {0}: {1}. ", httpCode, httpException.Message)));
                            Response.Redirect("/Error/http500");
                            return;
                        }
                    default:
                        {
                            Log.File.InfoFormat(_error.Msg.GenerateLogMsg(string.Format("Error {0}: {1}. ", httpCode, httpException.Message)));
                            Log.File.ErrorFormat(_error.Msg.GenerateLogMsg(string.Format("Error {0}: {1}. ", httpCode, httpException.Message)));
                            
                            return;
                        }
                }
            }
            else
            {
                Log.File.ErrorFormat(_error.Msg.GenerateLogMsg(string.Format("Unhandled error on page. Error:{0} {1}{2}", curException.Message,curException.InnerException,curException.Source)));
                Server.ClearError();
                Response.Redirect("/Error/Unhandled");
            }
        }

        void Application_PreSendRequestHeaders(Object sender, EventArgs e)
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
        }
    }
}