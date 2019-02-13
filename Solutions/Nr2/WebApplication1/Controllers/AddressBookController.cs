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

        private string GetCreatedDate()
        {
            return User.Identity.Name.Split(new string[] { "---NewInfo---" }, StringSplitOptions.None)[2];
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult LogIn()
        {
            ViewBag.ErrorMessage = "";
            ViewBag.CorrectUserName = true;
            ViewBag.CorrectPassword = true;
            return View(new User("", "", null));
        }

        [AllowAnonymous]
        public ActionResult LogInAnonymous()
        {
            DateTime createdDate;
            bool correctUserName, correctPassword;
            string errorMessage;

            Security.CheckUser(new User("Anonymous", "abc", null), out int userId, out createdDate, out correctUserName, out correctPassword, out errorMessage);
            string authCookieUserName = string.Format("{0}---NewInfo---{1}---NewInfo---{2}", userId.ToString(), "Anonymous", createdDate.ToString("yyyy-MM-dd"));
            FormsAuthentication.SetAuthCookie(authCookieUserName, false);
            return RedirectToAction("GetAll");
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult LogIn(User user)
        {
            DateTime createdDate;
            bool correctUserName, correctPassword;
            string errorMessage;

            Security.CheckUser(user, out int userId, out createdDate, out correctUserName, out correctPassword, out errorMessage);

            if (correctUserName && correctPassword)
            {
                string authCookieUserName = string.Format("{0}---NewInfo---{1}---NewInfo---{2}", userId.ToString(), user.Name, createdDate.ToString("yyyy-MM-dd"));
                FormsAuthentication.SetAuthCookie(authCookieUserName, false);
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

        public ActionResult GetAll()
        {
            string errorMessage;

            List<PersonInfo> list = PersonInfoDb.GetAll(GetUserId(), out errorMessage);

            if (errorMessage == null)
            {
                ViewBag.UserName = GetUserName();
                ViewBag.CreatedDate = GetCreatedDate();
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