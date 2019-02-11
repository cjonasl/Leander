using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [Authorize]
    public class AddressBookController : Controller
    {
        [AllowAnonymous]
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult Login(User user)
        {
            return View();
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult LoginAnonymous()
        {
            return View();
        }

        public ActionResult GetAll()
        {
            string errorMessage;

            List<PersonInfo> list = PersonInfoDb.GetAll(out errorMessage);

            if (errorMessage == null)
                return View("AddressBook", list);
            else
                return Json(errorMessage, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSingle(int id)
        {
            string errorMessage;

            PersonInfo person = PersonInfoDb.GetSingle(id, out errorMessage);

            if (errorMessage == null)
                return Json(person, JsonRequestBehavior.AllowGet);
            else
                return Json(errorMessage, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Add(PersonInfo person)
        {
            string errorMessage;

            int id = PersonInfoDb.Add(person, out errorMessage);

            if (errorMessage == null)
                return Json(id, JsonRequestBehavior.AllowGet);
            else
                return Json(errorMessage, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Update(PersonInfo person)
        {
            string errorMessage;

            PersonInfoDb.Update(person, out errorMessage);

            if (errorMessage == null)
                return Json("Success", JsonRequestBehavior.AllowGet);
            else
                return Json(errorMessage, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Delete(int id)
        {
            string errorMessage;

            PersonInfoDb.Delete(id, out errorMessage);

            if (errorMessage == null)
                return Json("Success", JsonRequestBehavior.AllowGet);
            else
                return Json(errorMessage, JsonRequestBehavior.AllowGet);
        }
    }
}