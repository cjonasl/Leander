using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApplication2.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AAA()
        {
            return View();
        }

        public ActionResult BBB()
        {
            return View();
        }

        public ActionResult CCC()
        {
            return View();
        }

        public ActionResult Jonas()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult Daniel()
        {
            return View();
        }

    }
}