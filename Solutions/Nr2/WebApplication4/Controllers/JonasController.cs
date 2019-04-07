using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApplication4.Controllers
{
    public class Person
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    public class JonasController : Controller
    {
        // GET: Jonas
        public ViewResult Index(string country = "Sweden")
        {
            return View(viewName: "Index", model: country);
        }

        public ViewResult Tennis(string country = "Sweden")
        {
            return View(viewName: "Tennis", model: country);
        }

        public ViewResult Badminton(Person person)
        {
            return View(viewName: "Badminton", model: person);
        }
    }
}