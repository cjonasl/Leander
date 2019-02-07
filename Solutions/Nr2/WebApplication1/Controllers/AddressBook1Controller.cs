using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class AddressBook1Controller : Controller
    {
        public ActionResult GetAddressBook()
        {
            return View(PersonInfo1Db.GetPersonInfos());
        }
    }
}