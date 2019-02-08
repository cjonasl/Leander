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
        public ActionResult GetAll()
        {
            string errorMessage;

            List<PersonInfo1> list = PersonInfo1Db.GetAll(out errorMessage);

            if (errorMessage == null)
                return View("AddressBook", list);
            else
                return Json(errorMessage, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSingle(int id)
        {
            string errorMessage;

            PersonInfo1 person = PersonInfo1Db.GetSingle(id, out errorMessage);

            if (errorMessage == null)
                return Json(person, JsonRequestBehavior.AllowGet);
            else
                return Json(errorMessage, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Add(PersonInfo1 person)
        {
            string errorMessage;

            int id = PersonInfo1Db.Add(person, out errorMessage);

            if (errorMessage == null)
                return Json(id, JsonRequestBehavior.AllowGet);
            else
                return Json(errorMessage, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Update(PersonInfo1 person)
        {
            string errorMessage;

            PersonInfo1Db.Update(person, out errorMessage);

            if (errorMessage == null)
                return Json("Success", JsonRequestBehavior.AllowGet);
            else
                return Json(errorMessage, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Delete(int id)
        {
            string errorMessage;

            PersonInfo1Db.Delete(id, out errorMessage);

            if (errorMessage == null)
                return Json("Success", JsonRequestBehavior.AllowGet);
            else
                return Json(errorMessage, JsonRequestBehavior.AllowGet);
        }
    }
}