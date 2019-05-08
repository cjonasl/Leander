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
        private static void Print(string msg)
        {
            System.IO.FileStream fs = new System.IO.FileStream("C:\\tmp\\OnActionExecuted.txt", System.IO.FileMode.Append, System.IO.FileAccess.Write);
            System.IO.StreamWriter sw = new System.IO.StreamWriter(fs, System.Text.Encoding.UTF8);
            sw.WriteLine(msg);
            sw.Flush();
            fs.Flush();
            sw.Close();
            fs.Close();
        }
        
        
        /// <summary>
        /// Execute filter
        /// </summary>
        /// <param name="filterContext">Filter context</param>
        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            var _store = new StoreService(Data);
            var controller = filterContext.RouteData.Values["controller"].ToString();

            string controllerAction = string.Format("(controller, action) = ({0}, {1})", filterContext.ActionDescriptor.ControllerDescriptor.ControllerName, filterContext.ActionDescriptor.ActionName);
            
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
                                {
                                    CheckStoreFilter.Print(controllerAction + " - new RedirectResult(\"/User/SignIn\");");
                                    filterContext.Result = new RedirectResult("/User/SignIn");
                                }
                                else
                                {
                                    CheckStoreFilter.Print(controllerAction + " - Direct exit");
                                }
                            }
                            else
                            {
                                CheckStoreFilter.Print(controllerAction + " - Direct exit");
                            }
                        }
                        else
                        {
                            CheckStoreFilter.Print(controllerAction + " - Direct exit");
                        }
                    }
                    else if (!action.Equals("NextStep") && !action.Equals("PreviousStep"))
                    {
                        CheckStoreFilter.Print(controllerAction + " - new RedirectResult(\"/User/SignIn\");");
                        filterContext.Result = new RedirectResult("/User/SignIn");
                    }
                    else
                    {
                        CheckStoreFilter.Print(controllerAction + " - Direct exit");
                    }
                }
                else if ((!action.Equals("SignIn")) && (!string.IsNullOrEmpty(action)))
                {
                    CheckStoreFilter.Print(controllerAction + " - new RedirectResult(Url.Process(PredefinedProcess.SignIn))");
                    filterContext.Result = new RedirectResult(Url.Process(PredefinedProcess.SignIn));
                }
                else
                {
                    CheckStoreFilter.Print(controllerAction + " - Direct exit");
                }
            }
            else
            {
                CheckStoreFilter.Print(controllerAction + " - Direct exit");
            }
            //else if ((!_store.IsStoreInfoExist()) && !controller.Equals("Product"))
            //{}
        }

    }
}