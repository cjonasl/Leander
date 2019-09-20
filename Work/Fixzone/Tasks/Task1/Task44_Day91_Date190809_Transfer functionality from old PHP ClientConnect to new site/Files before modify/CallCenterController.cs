using ClientConnect.Configuration;
using ClientConnect.Models.Store;
using ClientConnect.Services;
using ClientConnect.Validation;
using ClientConnect.ViewModels.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using PagedList;
using PagedList.Mvc;
using System.Web.Optimization;
using ClientConnect.Infrastructure;
using ClientConnect.Process;
using ClientConnect.Services;
namespace ClientConnect.Controllers
{
    public class CallCenterController : Controller
    {
        AccountService  accService;
        private Validator Validator;
        List<StoreInfoModel> ClientList;
        StoreService _storeService ;
        private ProcessService ProcessService { get; set; }
        public CallCenterController()
        {
            ClientList = new List<StoreInfoModel>();
            accService = (AccountService)Ioc.Get<AccountService>();
            Validator = (Validator)Ioc.Get<Validator>();
            _storeService = new StoreService();
            ProcessService = (ProcessService)Ioc.Get<ProcessService>();
        }


        public ActionResult ClientSearch(string Searchby, string search, int? page)
        {
            ClientList = _storeService.ClientList();
            if (Searchby == "ClientID")
            {
                var model = ClientList.Where(x => search.StartsWith(x.StoreId.ToString()) || search == null).ToList().ToPagedList(page ?? 1, 30);
                return View("Index",model);

            }
            else if (Searchby == "Name")
            {
                var model = ClientList.Where(x => x.StoreName.ToLower().Contains(search.ToLower()) || search == null).ToList().ToPagedList(page ?? 1, 30);
                return View("Index",model);
            }
            else
            {
                var model = ClientList.ToPagedList(page ?? 1, 30);
                return View("Index",model);
            }
        }

        public ActionResult Index(int? page=1)
        {
           
            ClientList = _storeService.ClientList();
            Response.Cookies["CC_StoreNumber"].Value = "0";
           
            if (!System.Web.HttpContext.Current.User.Identity.IsAuthenticated)

                return RedirectToAction("SignIn");
            else
            {
                var model = ClientList.ToPagedList(page.Value, 30);
                return View(model);
            }
        }
        //public ActionResult ClientIndex(int clientid)
        //{


        //    Response.Cookies["CC_StoreNumber"].Value = clientid.ToString();

        //    if (!System.Web.HttpContext.Current.User.Identity.IsAuthenticated)

        //        return RedirectToAction("SignIn");
        //    else
        //    {
        //        var model = ClientList.ToPagedList(page.Value, 30);
        //        return View(model);
        //    }
        //}
        public ActionResult GoToClient(int ClientId)
        {
            ProcessService.ClearCache();

            return RedirectToAction("ClientHome", "Home", new { ClientId = ClientId });
        }
         [HttpGet]
        public ActionResult SignIn()
        {

            var model = new AccountViewModel();
            return View(model);
           
        }
         [HttpPost]
         public ActionResult SignIn(AccountViewModel model)
         { var ValidRegex = new Regex(@"^[A-Za-z0-9 _]*$");
            if (ValidRegex.IsMatch(model.UserId))
            {
                var accDetails = accService.GetCallcenterAccountDetails(model.UserId);
                if (string.IsNullOrEmpty(model.Password) || string.IsNullOrEmpty(model.UserId))
                {
                    ModelState.AddModelError("Password", "Login Failed");
                    return View(model);
                } 
                if (accDetails != null)
                {
                    if (!model.Password.Equals(accDetails.UserPassword) && !string.IsNullOrEmpty(accDetails.UserPassword))
                    {
                        ModelState.AddModelError("Password", "Login Failed");
                        return View(model);
                    }
                    if (!accDetails.IsCallCenterUser)
                    {
                        ModelState.AddModelError("Password", "Your account has not been set as call center user.");
                        return View(model);
                    }
                    if (accDetails.Enabled==0)
                    {
                        ModelState.AddModelError("Password", "Your account has been disabled.");
                        return View(model);
                    }
                    if (ModelState.IsValid)
                    {
                        //Response.SetAuthCookie(model.UserId, true, accDetails.UserName);
                        accService.Authenticate(accDetails.UserId); accService.SessionInfo.UserId = model.UserId;
                        if (!accDetails.IsCallCenterUser)
                            return RedirectToAction("ClientIndex");
                        else
                        return RedirectToAction("Index");
                        //return Redirect(model.ReturnUrl);
                    }
                }
                else
                {
                    ModelState.AddModelError("Password", "Login Failed");
                    return View(model);
                }
            }
            else
            {
                ModelState.AddModelError("UserId", "An invalid Userid has been specified");
            }
            return View(model);
         }
    }
}
