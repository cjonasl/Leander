using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Security;

namespace AddressBook
{
    [Authorize]
    public class AddressBookController : Controller
    {
        private int GetUserId()
        {
            return int.Parse(User.Identity.Name.Split(new string[] { "---NewInfo---" }, StringSplitOptions.None)[0]);
        }

        private string GetUserName()
        {
            return User.Identity.Name.Split(new string[] { "---NewInfo---" }, StringSplitOptions.None)[1];
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult LogIn()
        {
            ViewBag.ErrorMessage = "";
            ViewBag.CorrectUserName = true;
            ViewBag.CorrectPassword = true;
            return View(new User("", ""));
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult LogIn(User user)
        {
            bool correctUserName, correctPassword;
            string errorMessage;

            Security.CheckUser(user, out int userId, out correctUserName, out correctPassword, out errorMessage);

            if (correctUserName && correctPassword)
            {
                ViewBag.UserName = user.Name;
                FormsAuthentication.SetAuthCookie(userId.ToString() + "---NewInfo---" + user.Name, false);
                return RedirectToAction("GetAll");
            }
            else
            {
                if (!correctUserName)
                    user.Name = "";

                ViewBag.ErrorMessage = errorMessage;
                ViewBag.CorrectUserName = correctUserName;
                ViewBag.CorrectPassword = correctPassword;
            }

            return View(user);
        }

        [AllowAnonymous]
        [HttpPost]
        public JsonResult CreateNewAccount(User user)
        {
            string errorMessage;

            Security.CreateNewAccount(user, out errorMessage);

            if (errorMessage == null)
                return Json("Success", JsonRequestBehavior.AllowGet);
            else
                return Json(errorMessage, JsonRequestBehavior.AllowGet);
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

            List<PersonInfo> list = PersonInfoDb.GetAll(GetUserId(), out errorMessage);

            if (errorMessage == null)
            {
                ViewBag.UserName = GetUserName();
                return View("AddressBook", list);
            }        
            else
                return Json(errorMessage, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSingle(int personId)
        {
            string errorMessage;

            PersonInfo person = PersonInfoDb.GetSingle(GetUserId(), personId, out errorMessage);

            if (errorMessage == null)
                return Json(person, JsonRequestBehavior.AllowGet);
            else
                return Json(errorMessage, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Add(PersonInfo person)
        {
            string errorMessage;

            int personId = PersonInfoDb.Add(GetUserId(), person, out errorMessage);

            if (errorMessage == null)
                return Json(personId, JsonRequestBehavior.AllowGet);
            else
                return Json(errorMessage, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Update(PersonInfo person)
        {
            string errorMessage;

            PersonInfoDb.Update(GetUserId(), person, out errorMessage);

            if (errorMessage == null)
                return Json("Success", JsonRequestBehavior.AllowGet);
            else
                return Json(errorMessage, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Delete(int personId)
        {
            string errorMessage;

            PersonInfoDb.Delete(GetUserId(), personId, out errorMessage);

            if (errorMessage == null)
                return Json("Success", JsonRequestBehavior.AllowGet);
            else
                return Json(errorMessage, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ChangePassword(ChangePassword changePassword)
        {
            string errorMessage;

            Security.ChangePassword(GetUserName(), changePassword, out errorMessage);

            if (errorMessage == null)
                return Json("Success", JsonRequestBehavior.AllowGet);
            else
                return Json(errorMessage, JsonRequestBehavior.AllowGet);
        }

        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("LogIn");
        }
    }
}