using System;
using System.Web.Mvc;
using System.Web.Security;
using ClientConnect.Configuration;
using ClientConnect.Infrastructure;
using ClientConnect.Models.Account;
using ClientConnect.Process;
using ClientConnect.Services;
using ClientConnect.Validation;
using ClientConnect.Validation.Account;
using ClientConnect.ViewModels.Account;
using FluentValidation;
using Omu.ValueInjecter;
using System.Text.RegularExpressions;

namespace ClientConnect.Controllers
{
    public class AccountController : Controller
    {
        private AccountService accService;
        private Validator Validator { get; set; }

        /// <summary>
        /// Service
        /// </summary>
        public AccountController()
        {
            accService = (AccountService)Ioc.Get<AccountService>();
            Validator = (Validator)Ioc.Get<Validator>();
        }

        ////
        //// GET: /Account/
        [HttpGet]
        public ActionResult SignIn(string returnUrl)
        {
            if (string.IsNullOrEmpty(returnUrl))
                returnUrl = Request.UrlReferrer==null ? "CallCenter/Signin":Request.UrlReferrer.ToString();
            
            var model = new AccountViewModel{ReturnUrl = returnUrl};
            return View(model);
        }

        [HttpPost]
        public ActionResult SignIn(AccountViewModel model)
        {
            var ValidRegex = new Regex(@"^[A-Za-z0-9 _]*$");
            if (ValidRegex.IsMatch(model.UserId))
            {

                var accDetails = accService.GetAccountDetails(model.UserId);
                model.IsDiagnosted = accService.IsDiagnosted;
                //validation
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
                    if (accDetails.ClientDisabled)
                    {
                        ModelState.AddModelError("Password", "Your account has been disabled.");
                        return View(model);
                    }
                    if (accDetails.ClientOnStopFg)
                    {
                        ModelState.AddModelError("Password", "Your account has been put on stop. " + accDetails.ClientOnStopNotes);
                        return View(model);
                    }
                }
                else
                {
                    ModelState.AddModelError("Password", "Login Failed");
                    return View(model);
                }


                foreach (var error in new AccountDetailsValidation().Validate(accDetails, ruleSet: ValidationRuleSets.defaultRule).Errors)
                {
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }

                //redirections
                if (accDetails.PasswordExpired)
                {
                    accService.SessionInfo.UserId = model.UserId;
                    return Redirect(Url.Process(PredefinedProcess.ExpiredPassword));
                }

                if (string.IsNullOrEmpty(accDetails.UserPassword))
                {
                    accService.SessionInfo.UserId = model.UserId;
                    return Redirect(Url.Process(PredefinedProcess.UserEmptyPassword));
                }


                //confirm message
                if (!model.IsMessageConfirmed)
                {
                    if (string.IsNullOrEmpty(accService.CookiesStoredInfo.StoreId))
                    {
                        model.InjectFrom(accDetails);
                        model.ShowNoStoremMessage = true;
                        return View(model);
                    }
                    if (!accService.CookiesStoredInfo.StoreId.Equals(accDetails.UserStoreId.ToString()))
                    {
                        model.InjectFrom(accDetails);
                        model.ShowDifferentUserMessage = true;
                        return View(model);
                    }
                }

                if (ModelState.IsValid)
                {
                    //Response.SetAuthCookie(model.UserId, true, accDetails.UserName);
                    accService.Authenticate(accDetails.UserId); accService.SessionInfo.UserId = model.UserId;
                    return Redirect(Url.ProcessLastMain());
                    //return Redirect(model.ReturnUrl);
                }

            }
            else
            {
                ModelState.AddModelError("UserId", "An invalid Userid has been specified");
            }
            return View(model);
        }
        
        /// <summary>
        /// Sign out
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult SignOut()
        {
            // clear authentication cookie
            FormsAuthentication.SignOut();
            //HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, "");
            //cookie.Expires = DateTime.Now.AddYears(-1);
            //Response.Cookies.Add(cookie);

            //HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            //FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(authCookie.Value);

            //Response.SetAuthCookie(model.UserId,
            //            true, "Test store");
            return Redirect("/");
        }

        /// <summary>
        /// Confidential info
        /// </summary>
        /// <returns>View</returns>
        [HttpGet]
        public ActionResult ChangePassword(string userId, string returnUrl)
        {
            if (accService.SessionInfo == null || string.IsNullOrEmpty(accService.SessionInfo.UserId))
            {
                return Redirect("/");
            }
            ViewBag.UserId = accService.SessionInfo.UserId;
            ViewBag.ReturnUrl = returnUrl ?? string.Empty;
            return View();
        }

        /// <summary>
        /// Confidential info
        /// </summary>
        /// <returns>View</returns>
        [HttpPost]
        public ActionResult ChangePassword(string userId, string newPassword, string returnUrl)
        {
            //validation
            if (string.IsNullOrEmpty(newPassword))
            {
                ModelState.AddModelError("NewPassword", "Input password");
            }

            var accDetails = accService.GetAccountDetails(userId);
            if (accDetails != null)
            {
                if (newPassword != null && newPassword.Equals(accDetails.UserPassword))
                {
                    ModelState.AddModelError("NewPassword", "You have already used this password, please choose a new one.");
                }

                if (ModelState.IsValid)
                {
                    accService.UpdatePassword(userId, newPassword);
                    accService.Authenticate(accDetails.UserId);
                    return Redirect(Url.ProcessLastMain());
                }
            }
            else ModelState.AddModelError("NewPassword", "Can't find user id.");

            ViewBag.UserId = userId;
            ViewBag.ReturnUrl = returnUrl ?? string.Empty;
            return View(model: newPassword);

        }
        public ActionResult CallCenterAccountWidget()
        {
            var storedInfo = accService.CookiesStoredInfo;
            if (string.IsNullOrEmpty(storedInfo.StoreId))
            {
               // ViewBag.StoreInfo = "No store selected";
                return PartialView();
            }

            //if (storedInfo.IsCallCenter)
            //{
            //    ViewBag.StoreInfo = "Call Center";
            //    return PartialView();
            //}

            ViewBag.StoreInfo = string.Format("{0} - {1}", storedInfo.StoreId, storedInfo.StoreName);
            return PartialView();
        }
        public ActionResult AccountWidget()
        {
            var storedInfo = accService.CookiesStoredInfo;
            if (string.IsNullOrEmpty(storedInfo.StoreId))
            {
                ViewBag.StoreInfo = "No store selected";
                return PartialView();
            }

            //if (storedInfo.IsCallCenter)
            //{
            //    ViewBag.StoreInfo = "Call Center";
            //    return PartialView();
            //}

            ViewBag.StoreInfo = string.Format("{0} - {1}", storedInfo.StoreId, storedInfo.StoreName);
            return PartialView();
        }
        
        /// <summary>
        /// First step - New User first entering. 
        /// </summary>
        /// <returns>Show DateBirth form</returns>
        [HttpGet]
        public ActionResult DateOfBirthCheck(string userId, string returnUrl)
        {
            if (accService.SessionInfo == null || string.IsNullOrEmpty(accService.SessionInfo.UserId))
            {
                return Redirect("/");
            }

            ViewBag.UserId = accService.SessionInfo.UserId;
            ViewBag.ReturnUrl = returnUrl ?? string.Empty;
            return View(DateTime.Now);
        }

        /// <summary>
        /// Get and check DateOfBirth
        /// </summary>
        /// <param name="dateOfBirth">Model with the date of birth</param>
        /// <returns> Date birth wrong - previous step (false) 
        ///  right - process next step  </returns>
        [HttpPost]
        public ActionResult DateOfBirthCheck(string userId, string returnUrl, DateTime? dateOfBirth)
        {
            //validation
            if (dateOfBirth != null)
            {
                var accDetails = accService.GetAccountDetails(userId);
                if (!dateOfBirth.Equals(accDetails.DateOfBirth))
                {
                    ModelState.AddModelError("dateOfBirth", "Wrong date of birth");
                }

                //if success
                if (ModelState.IsValid)
                {
                    return Redirect(Url.ProcessNextStep());
                }
            }
            else ModelState.AddModelError("dateOfBirth", "Input date of birth");
            

            ViewBag.UserId = userId;
            ViewBag.ReturnUrl = returnUrl ?? string.Empty;
            return View(dateOfBirth ?? DateTime.Now);
        }

        /// <summary>
        /// Confidential info
        /// </summary>
        /// <returns>View</returns>
        [HttpGet]
        public ActionResult ConfidentialDetails(string returnUrl)
        {
            //if (accService.SessionInfo == null || string.IsNullOrEmpty(accService.SessionInfo.UserId))
            //{
            //    return Redirect("/");
            //}
            var model = new AccountConfidentialInfo();
            ViewBag.UserId = System.Web.HttpContext.Current.User.Identity.Name;
            ViewBag.ReturnUrl = returnUrl ?? string.Empty;
            return View(model);
        }

        /// <summary>
        /// Confidential info
        /// </summary>
        /// <returns>View</returns>
        [HttpPost]
        public ActionResult ConfidentialDetails(AccountConfidentialInfo model, string userId, string returnUrl)
        {
            //validation       
            var result = new AccountConfidentialInfoValidation().Validate(model, ruleSet: ValidationRuleSets.defaultRule);
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
            }
            if (ModelState.IsValid)
            {
                accService.UpdateCofirmInfo(userId,  model.ReminderQuestion, model.ReminderAnswer);
                accService.Authenticate(userId);
                return Redirect(Url.ProcessLastMain());
            }
            ViewBag.UserId = userId;
            ViewBag.ReturnUrl = returnUrl ?? string.Empty;
            return View(model);
        }

        /// <summary>
        /// User id input form
        /// </summary>
        /// <returns>View</returns>
        [HttpGet]
        public ActionResult InputUserId(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl ?? string.Empty;
            return View();
        }

        /// <summary>
        /// User id check
        /// </summary>
        /// <param name="userId">User id</param>
        /// <returns>Next step or error message</returns>
        [HttpPost]
        public ActionResult InputUserId(string userId, string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl ?? string.Empty;
            var accDetails = accService.GetAccountDetails(userId);
            //validation
            if (accDetails == null || string.IsNullOrEmpty(accDetails.UserId))
            {
                ModelState.AddModelError("UserId", "User not exist.");
                return View(model: userId);
            }
            if (accDetails.Enabled == 0)
            {
                ModelState.AddModelError("UserId", "User disabled.");
                return View(model: userId);
            }

            if (ModelState.IsValid)
            {
                accService.SessionInfo.UserId = userId;
                return Redirect(Url.ProcessNextStep());
            }
            
            return View(model: userId);
        }

        /// <summary>
        /// Reminder question
        /// </summary>
        /// <returns>View</returns>
        [HttpGet]
        public ActionResult ReminderAnswer(string userId, string returnUrl)
        {
            if (accService.SessionInfo == null || string.IsNullOrEmpty(accService.SessionInfo.UserId))
            {
                return Redirect("/");
            }
            var accDetails = accService.GetAccountDetails(accService.SessionInfo.UserId);
            ViewBag.UserId = accService.SessionInfo.UserId;
            ViewBag.ReturnUrl = returnUrl ?? string.Empty;
            ViewBag.Question = accDetails == null ? "" : accDetails.ReminderQuestion;
            return View();
        }

        /// <summary>
        /// Reminder question
        /// </summary>
        /// <returns>Next step or error message</returns>
        [HttpPost]
        public ActionResult ReminderAnswer(string answer, string userId, string returnUrl)
        {
            var accDetails = accService.GetAccountDetails(userId);
            if (string.IsNullOrEmpty(answer) || !answer.Trim().Equals(accDetails.ReminderAnswer.Trim()))
            {
                ModelState.AddModelError("answer", "Wrong answer.");
            }
            if (ModelState.IsValid)
            {
                return Redirect(Url.Process(PredefinedProcess.ExpiredPassword));
            }
            ViewBag.UserId = userId;
            ViewBag.ReturnUrl = returnUrl ?? string.Empty;
            ViewBag.Question = accDetails.ReminderQuestion;
            return View(model: answer);
        }

        /// <summary>
        /// Page if access denied
        /// </summary>
        /// <param name="role">Role name for message</param>
        /// <returns></returns>
        public ActionResult AccessDenied(string role = "Administrator")
        {
            return View(model: role);
        }
    }
}
