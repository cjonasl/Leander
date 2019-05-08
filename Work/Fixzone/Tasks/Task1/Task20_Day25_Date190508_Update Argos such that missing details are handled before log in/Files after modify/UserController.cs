using System;
using System.Web.Mvc;
using System.Web.Security;
using CAST.Services;
using CAST.Process;
using CAST.ViewModels.User;



namespace CAST.Controllers
{
    /// <summary>
    /// User acounts controller
    /// </summary>
    public class UserController : DataController
    {
        /// <summary>
        /// property for getting store info 
        /// </summary>
        private readonly UserService _userService;

        /// <summary>
        /// property for getting store info 
        /// </summary>
        private readonly StoreService _storeService;

        public UserController()
        {
            _userService = new UserService(Data);
            _storeService = new StoreService(Data);
        }


        #region SignIn
        
        /// <summary>
        /// User checking
        /// </summary>
        /// <returns>Redirect to page</returns>
        public ActionResult SignIn(bool notAdmin = false)
        {
           
                // Save Url for return
            if (!_userService.IsAutoSignIn())
            {
                if (Request.UrlReferrer != null)
                {
                    if (Request.UrlReferrer.ToString().IndexOf("SignIn") <= 0)
                        _userService.SetUrlForBack(Request.UrlReferrer != null ? Request.UrlReferrer.ToString() : "/");
                }
                else
                    _userService.SetUrlForBack("/");
            }
            else
            {
                _userService.SetUrlForBack(Request.UrlReferrer != null ? Request.UrlReferrer.ToString() : "/");
            }
            var model = new User_DetailsModel { UserId = String.Empty };

            // if admin error 
            if (notAdmin)
            {
                ViewBag.userNotAdminMsg = "You must be administrator.";
            }
            return View("SignIn", model);
        }

        /// <summary>
        /// User sign in
        /// </summary>
        /// <param name="userModel">Model of user info (employee number and password)</param>
        /// <returns>Redirect on the same page or on referer page</returns>
        [HttpPost]
        public ActionResult SignIn(User_DetailsModel model)
        {
            // get user info 
            var userInfo = _userService.GetUserInfo(model.UserId, model.UserPassword);
            userInfo.UserComputerName = model.UserComputerName;
            userInfo.UserMemoryAvailable = model.UserMemoryAvailable;
            userInfo.UserPassword = model.UserPassword;
         
            // If user id exist in DB
            if (!string.IsNullOrEmpty(userInfo.UserId))
            {
                // If user disabled
                if (!Convert.ToBoolean(userInfo.Enabled))
                {
                    int days = 30;

                    if (userInfo.DisabledDate.HasValue)
                        days = (DateTime.Today - userInfo.DisabledDate.Value).Days;

                    string errorMessage = string.Format("Account inactive for more than 90 days, please request re-activation by your manager. If no further login in next {0} days you will be deleted from the system.", days);

                    ModelState.AddModelError("UserId", errorMessage);
                    return View(userInfo);
                }

                //if auto diagnose
                if (Convert.ToBoolean(userInfo.RunAutoDiagnostic))
                {
                    return View(userInfo);
                }

                // if password in DB is empty
                if (Convert.ToBoolean(userInfo.IsPasswordEmpty) )
                {
                    _userService.SetFirstTimeUserId(userInfo.UserId);
                    return Redirect(Url.Process(PredefinedProcess.FirstTimeNewUser));
                }

                // This is made for clean back url from session and flag
                string urlForback = _userService.GetUrlForBack() ?? "/";
                bool IsAutoSignIn = _userService.IsAutoSignIn();

                // Check if paswword is expired, or if ReminderQuestion, ReminderAnswer and/or DateOfBirth need to be set
                if (userInfo.PasswordExpired || string.IsNullOrEmpty(userInfo.ReminderQuestion) || string.IsNullOrEmpty(userInfo.ReminderAnswer) || !userInfo.DateOfBirth.HasValue)
                {
                    _userService.SetFirstTimeUserId(userInfo.UserId);
                    HttpContext.Session["signInUserInfo"] = userInfo;
                    return Redirect(Url.Process(PredefinedProcess.ExpiredPassword));
                }

                _userService.SetAuthInfo(userInfo);

                // Redirect back
                if (IsAutoSignIn && !urlForback.Contains("/"))
                {
                    return Redirect(Url.Process(Convert.ToInt32(urlForback)));
                }

                // If auto sign in false, then clear self process
                var process = new ProcessController();
                process.RemoveCurrentProcess();

                return Redirect(urlForback);
            }
            userInfo.RunAutoDiagnostic = 0;
            userInfo.UserId = model.UserId;
            
            ModelState.AddModelError("UserId", "User not found! Check login and password.");
          
            return View(userInfo);
        }
        
        /// <summary>
        /// Return is need to show window with notification
        /// </summary>
        /// <param name="user">Inputed user name</param>
        /// <param name="password">Inputed user password</param>
        /// <returns>Json data - flags for each window</returns>
        public JsonResult DisplayConfirmMessage(string user, string password)
        {
            // Set default values
            var ShowWinNoStore = false;
            var ShowWinOtherUser = false;

            string msg = "";

            // If password or user not inputed get user store number
            if (!string.IsNullOrEmpty(user) && !string.IsNullOrEmpty(password))
            {
                var model = _userService.GetUserInfo(user, password);
                if (model.UserId != null)
                {
                    // If no store, set flag in true
                    if (!_storeService.IsStoreInfoExist())
                    {
                        ShowWinNoStore = true;
                        msg = String.Format("Set the Store for CAST. <br/>" +
                                            "The store has not been set on CAST for this computer. Once you login the store you are assigned to <b>{0}</b> will be set on this computer. If this is <span class=\"error\"><b>NOT THE STORE</b></span> you are currently in please click ‘No’ and ask another colleague from the store to login first." +
                                            "Are you in the Argos store {1}?", model.UserStoreName, model.UserStoreName);
                    }
                    else
                    {
                        // If other user set flag in true
                        if (_storeService.GetStoreId() != model.UserStoreID)
                        {
                            ShowWinOtherUser = true;
                            msg = String.Format("Do you wish to change your store to {0}?", _storeService.GetStoreName());
                                //"Please Note \n" +
                                //                "CAST is set to store {0} on this computer. Your login is assigned to {1}. If you have transferred stores please alert your manager that you need to be transferred from {2} to {3} in CAST.",
                                //                _storeService.GetStoreName(), model.UserStoreName, model.UserStoreName,
                                //                _storeService.GetStoreName());
                        }

                    }
                }
            }
            return Json(new { ShowWinNoStore, ShowWinOtherUser, msg }, JsonRequestBehavior.AllowGet);
        }


        public JsonResult ChangeUserStoreNumber(string Userid,string storeNumber)
        {
            string msg="";string NewStore="";
            string newStoreId = _storeService.GetStoreId().ToString();
            if (newStoreId != storeNumber)
            {
                 _userService.UpdateUserStore(Userid, newStoreId);
                 msg = "success"; NewStore = newStoreId;
            }
            return Json(new { msg, newStoreId });
        }

        //public JsonResult DisplayPwdTxtforStoreUser(string user)
        //{
           
        //    string msg = "";
        //    var isStoreUser = false;
        //    string loginNB = ""; 
        //    loginNB = "Call centre colleagues should use their Windows user and password as usual";

        //         msg = "Password";
        //    // If password or user not inputed get user store number
        //    if (!string.IsNullOrEmpty(user))
        //    {
        //        isStoreUser = _userService.GetUserInfo(user);
                
                   
                
        //        if (isStoreUser)
        //        {
        //            loginNB = "Store colleagues should now use their assigned Argos store number rather than their password. Passwords will no longer be used";
        //            msg = "Your Store Number";
                    
        //        }

        //    }
        //    return Json(new { msg, loginNB, isStoreUser }, JsonRequestBehavior.AllowGet);
        //}

        private void LogOff()
        {
            _userService.SetUrlForBack("");
            _userService.ClearInfoFromSession();

            // NOTE: we save store details even if user logs out
            FormsAuthentication.SignOut();
        }

        /// <summary>
        /// Sign out
        /// </summary>
        /// <returns>Redirect to previous page</returns>
        public ActionResult SignOut()
        {
            LogOff();
            return RedirectToRoute("Default", new { controller = "Home", action = "Index", id = UrlParameter.Optional });
        }


        #endregion
         
        #region  force user to enter DOB and Security QA if it is not filled.
        [HttpGet]
        public ActionResult MissingDetails()
        {

            User_AccountInfoModel model = _userService.GetUserAccountInfo(_userService.GetFirstTimeUserId());
            return View(model);
        }

        [HttpGet]
        public ActionResult CheckDetails()
        {
            User_DetailsModel userInfo = (User_DetailsModel)HttpContext.Session["signInUserInfo"];

            if (string.IsNullOrEmpty(userInfo.ReminderQuestion) || string.IsNullOrEmpty(userInfo.ReminderAnswer) || !userInfo.DateOfBirth.HasValue)
                return View("MissingDetails", new User_AccountInfoModel() 
                { 
                    ReminderQuestion = userInfo.ReminderQuestion,
                    ReminderAnswer = userInfo.ReminderAnswer,
                    Day = userInfo.DateOfBirth.HasValue ? userInfo.DateOfBirth.Value.Day : 0,
                    Month = userInfo.DateOfBirth.HasValue ? userInfo.DateOfBirth.Value.Month : 0,
                    Year = userInfo.DateOfBirth.HasValue ? userInfo.DateOfBirth.Value.Year : 0

                });
            else
            {
                _userService.SetAuthInfo(userInfo);
                HttpContext.Session.Remove("signInUserInfo");
                return Redirect(Url.ProcessNextStep());
            }
        }

        [HttpPost]
        public ActionResult MissingDetails(User_AccountInfoModel model)
        {
            if (ModelState.IsValid)
            {
                bool? errorVariableIsYear;
                string errorMessage = CAST.Validation.DateOfBirthValidation.Check(model.Year, model.Month, model.Day, out errorVariableIsYear);

                if (!string.IsNullOrEmpty(errorMessage))
                {
                    if (errorVariableIsYear.Value)
                        ModelState.AddModelError("Year", errorMessage);
                    else
                        ModelState.AddModelError("Day", errorMessage);

                    return View(model);
                }

                model.UserId = _userService.GetFirstTimeUserId();
                model.DateOfBirth = new DateTime(model.Year, model.Month, model.Day);
                _userService.SaveUserDetails(model);

                User_DetailsModel userInfo = (User_DetailsModel)HttpContext.Session["signInUserInfo"];
                _userService.SetAuthInfo(userInfo);
                HttpContext.Session.Remove("signInUserInfo");

                return Redirect(Url.ProcessNextStep());
            }

            return View(model);
        }
        #endregion
        #region New User first logIn
        /// <summary>
        /// - First step - New User first entering. 
        /// </summary>
        /// <param name="evaluation">Flag for evaluation if user get wrong DateOfBirth</param>
        /// <returns>Show DateBirth form</returns>
        public ActionResult ShowDateBirth()
        {
            var model = new User_DateOfBirthModel();
            model.Day = 1;
            model.Month = 1;
            model.Year = 2000;

            return View(model);
        }

        /// <summary>
        /// Get and check DateOfBirth
        /// </summary>
        /// <param name="model">Model with the date of birth</param>
        /// <returns> Date birth wrong - previous step (false) 
        ///  right - process next step  </returns>
        [HttpPost]
        public ActionResult ShowDateBirth(User_DateOfBirthModel model)
        {
            try
            {
                var func = new FunctionsController();
                func.StringToDateTimeDDmmYYYY(model.Day, model.Month, model.Year);

                //check dateOfBirth as the password
                if (_userService.CheckUserDateOfBirth(_userService.GetFirstTimeUserId(), model))
                {
                    return Redirect(Url.ProcessNextStep());
                }
                ModelState.AddModelError("Day","Please contact your manager! The date entered does not match the date on the record!");
                ModelState.AddModelError("Month", " ");
                ModelState.AddModelError("Year", " ");
            }
            catch(Exception e)
            {
                ModelState.AddModelError("Day", "Wrong date");
                ModelState.AddModelError("Month", " ");
                ModelState.AddModelError("Year", " ");
            }
            return View(model);
        }

        /// <summary>
        ///  - Second step - New User first entering - Show reminder info form.
        /// </summary>
        /// <returns>View reminder question, answer, password</returns>
        public ActionResult ShowConfidentialDetails()
        {
            return View();
        }

        /// <summary>
        /// Saving secret information.
        /// </summary>
        /// <param name="model">model with secret info</param>
        /// <returns>redirest to signIn with filled model (pass, userId)</returns>
        [HttpPost]
        public ActionResult ShowConfidentialDetails(User_ConfidentialInfoModel model)
        {
            // Saving details
            if(ModelState.IsValid)
            {
                model.UserId = _userService.GetFirstTimeUserId();
                _userService.SaveUserDetails(model);
                var info = _userService.GetUserInfo(model.UserId, model.Password);
                
                // if user disabled
                if (!Convert.ToBoolean(info.Enabled))
                {
                    return RedirectToAction("SignIn");
                }

                // This is made for clean back url from session and flag
                string urlForback = _userService.GetUrlForBack() ?? "/";
                bool IsAutoSignIn = _userService.IsAutoSignIn();

                _userService.SetAuthInfo(info);
                
                // Redirect back
                if (IsAutoSignIn && !urlForback.Contains("/"))
                    return Redirect(Url.Process(Convert.ToInt32(urlForback)));
                return Redirect(urlForback);
            }
            return View(model);

        }
        #endregion

        #region RestorePassword

        /// <summary>
        ///  - First step - User change password
        /// </summary>
        /// <param name="isUserIdExist">is id exist - flag</param>
        /// <returns>View form for getting user ID</returns>
        public ActionResult ShowUserId()
        {
            var model = new User_RestorePassword();
            return View(model);
        }

        /// <summary>
        /// Check User Id
        /// </summary>
        /// <param name="userId">user Id</param>
        /// <returns>id exists YES - next process step. Save userId in session
        /// NO  - step back, flag false</returns>
        [HttpPost]
        public ActionResult ShowUserId(User_RestorePassword model)
        {
            if (_userService.IsUserIdExist(model.UserId))
            {
                // Check if user has empty password (first logIn)
                if (_userService.IsUserPasswordEmpty(model.UserId))
                {
                    _userService.SetFirstTimeUserId(model.UserId);
                    return Redirect(Url.Process(PredefinedProcess.FirstTimeNewUser));
                }
                _userService.SetFirstTimeUserId(model.UserId);
                return Redirect(Url.ProcessNextStep());
            }
            else ModelState.AddModelError("UserId","User id not found");
            return View(model);
        }

        /// <summary>
        /// - Second Step -User change password - getting reminder question
        /// </summary>
        /// <param name="isRightAnswer">Check answer - flag</param>
        /// <returns>View form for getting reminder answer</returns>
        public ActionResult ShowReminderQuestion()
        {
            // Get confirm Answer
            var model = new User_RestorePassword();
            model.ReminderQuestion = _userService.GetConfirmAnswer(_userService.GetFirstTimeUserId());
            if (string.IsNullOrEmpty(model.ReminderQuestion))
            {
                string message= "Security question is not set for your account. Please contact your manager to set your security question.";
                return RedirectToAction("ShowContactManager", new { message = message });
            }

            else
            return View(model);
        }

        //If no Reminder question is set., the manager should be contacted
        public ActionResult ShowContactManager(string message)
        {
            return View((object)message);
        }
        /// <summary>
        /// Checking reminder answer
        /// </summary>
        /// <param name="ReminderAnswer">answer on the question</param>
        /// <returns>answer right YES - next process step
        ///                       NO  - step back flag false</returns>
        [HttpPost]
        public ActionResult ShowReminderQuestion(User_RestorePassword model)
        {
            bool isRightAnswer = _userService.IsRightAnswerOnQuestion(model.ReminderAnswer, _userService.GetFirstTimeUserId());

            if (isRightAnswer) return Redirect(Url.ProcessNextStep());
            ModelState.AddModelError("ReminderAnswer","Wrong answer.");
            return View(model);
        }

        /// <summary>
        /// Third Step -User change password - Getting new password
        /// </summary>
        /// <returns>View form for getting new password</returns>
        public ActionResult NewPassword()
        {
            var model = new User_RestorePassword();
            return View(model);
        }

        /// <summary>
        /// Saving new password
        /// </summary>
        /// <param name="model">model with new password</param>
        /// <returns>it is the last step - return to the previous process</returns>
        [HttpPost]
        public ActionResult NewPassword(User_RestorePassword model)
        {
            if (ModelState.IsValid)
            {
                if (HttpContext.Session["NumberofAttemptsToSetPassword"] != null)
                    HttpContext.Session.Remove("NumberofAttemptsToSetPassword");

                if (string.IsNullOrEmpty(model.Password))
                {
                    ModelState.AddModelError("Password", "Input Password");
                }
                else
                {
                    bool isSamePassword = _userService.SaveNewPassword(_userService.GetFirstTimeUserId(), model.Password);
                    if (isSamePassword)
                    {
                        ModelState.AddModelError("Password",
                                                 "You have already used this password, please choose a new one.");
                    }
                    else
                        return Redirect(Url.ProcessNextStep());
                }
            }
            else
            {
                if (HttpContext.Session["NumberofAttemptsToSetPassword"] == null)
                    HttpContext.Session["NumberofAttemptsToSetPassword"] = 0;

                int numberofAttemptsToSetPassword = 1 + (int)HttpContext.Session["NumberofAttemptsToSetPassword"];

                if (numberofAttemptsToSetPassword == 3)
                {
                    _userService.DisableUser(_userService.GetFirstTimeUserId());
                    _userService.SetUrlForBack("");
                    _userService.ClearInfoFromSession();
                    HttpContext.Session.Remove("signInUserInfo");
                    ViewBag.ChangePasswordFailure = true;
                }
                else
                    HttpContext.Session["NumberofAttemptsToSetPassword"] = numberofAttemptsToSetPassword;
            }
            return View(model);
        }

        #endregion

        #region Expired Password
        /// <summary>
        /// - First step - password is expired
        /// _userState.FirstTimeUserId - user ID </summary>
        /// <returns></returns>
        public ActionResult ExpiredPassword()
        {
            User_DetailsModel userInfo = (User_DetailsModel)HttpContext.Session["signInUserInfo"];

            if (userInfo.PasswordExpired)
            {
                ViewBag.header = "Period of validity of the password expired. ";
                return View("NewPassword");
            }
            else
                return Redirect(Url.ProcessNextStep());
        }

        #endregion

        
    }
}
