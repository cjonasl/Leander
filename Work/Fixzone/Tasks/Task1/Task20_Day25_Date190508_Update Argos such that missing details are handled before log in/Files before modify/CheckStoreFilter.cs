using System;
using System.Web.Mvc;
using CAST.Controllers;
using CAST.Process;
using CAST.Services;

namespace CAST.UserAccount
{
    /// <summary>
    /// Filter for page access, if no store number
    /// </summary>
    public class CheckStoreFilter : DataController
    {
        /// <summary>
        /// Execute filter
        /// </summary>
        /// <param name="filterContext">Filter context</param>
        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            var _store = new StoreService(Data);
            var controller = filterContext.RouteData.Values["controller"].ToString();
            
            // Access denied to all page, but not to 'Home" and "User"
            if ((!_store.IsStoreInfoExist()) && !controller.Equals("Home") && !controller.Equals("User") && !controller.Equals("Diagnostic") && !controller.Equals("Functions"))
            {
                controller = filterContext.RouteData.Values["controller"].ToString();
                var action = string.Empty;

                // Allow access to proccess 'FirstTime"
                if (controller.Equals("Process"))
                {
                    var id = filterContext.RouteData.Values["id"];
                    action = filterContext.RouteData.Values["action"].ToString();

                    // If start process First time log in
                    if (action.Equals("Go"))
                    {
                        // First time log in process number
                        if (Convert.ToInt32(id) != (int)PredefinedProcess.FirstTimeNewUser)
                        {
                            if (Convert.ToInt32(id) != (int)PredefinedProcess.SignIn)
                            {
                                // Forgoten password process    
                                if (!(Convert.ToInt32(id) == Convert.ToInt32(PredefinedProcess.ForgottenPassword) || Convert.ToInt32(id) == Convert.ToInt32(PredefinedProcess.ProductSearch)))
                                    filterContext.Result = new RedirectResult("/User/SignIn");
                            }
                        }
                    }
                    else
                        if (!action.Equals("NextStep") && !action.Equals("PreviousStep"))
                        {
                            filterContext.Result = new RedirectResult("/User/SignIn");
                        }
                        
                }

                else if ((!action.Equals("SignIn"))&&(!string.IsNullOrEmpty(action)))
                {
                    filterContext.Result = new RedirectResult(Url.Process(PredefinedProcess.SignIn));
                }
            }
            //else if ((!_store.IsStoreInfoExist()) && !controller.Equals("Product"))
            //{}
        }

    }
}