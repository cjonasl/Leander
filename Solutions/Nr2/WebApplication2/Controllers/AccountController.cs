using System;
using System.Collections;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace WebApplication1.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet]
        [AllowAnonymous]
        public ActionResult SignIn()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult SignIn(string name)
        {
            FormsAuthentication.SetAuthCookie(name, false);

            ArrayList v = (ArrayList)HttpContext.Cache["SignedInUsers"];
            v.Add(name);

            v = (ArrayList)HttpContext.Cache["CachedResponses"];
            v.Add(new WebApplication2.CachedResponseWrapper());

            return RedirectToAction("Jonas", "Home");
        }

        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();

            ArrayList v = (ArrayList)HttpContext.Cache["SignedInUsers"];
            int index = v.IndexOf(HttpContext.User.Identity.Name);
            v.RemoveAt(index);
            v = (ArrayList)HttpContext.Cache["CachedResponses"];
            v.RemoveAt(index);

            return RedirectToAction("SignIn");
        }
    }
}