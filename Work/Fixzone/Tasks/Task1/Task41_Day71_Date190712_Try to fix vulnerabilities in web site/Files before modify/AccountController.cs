using System;
using System.Web.Mvc;
using System.Web.Security;
using CAST.Configuration;
using CAST.Infrastructure;
using CAST.Models.Account;
using CAST.Process;
using CAST.Services;
using CAST.Validation;
using CAST.Validation.Account;
using CAST.ViewModels.Account;
using FluentValidation;
using Omu.ValueInjecter;
using CAST.ViewModels.Administration;
using CAST.Validation.Administration;
using System.Text.RegularExpressions;
using System.Linq;
using DevOne.Security.Cryptography.BCrypt;
using CAST.Logging;

namespace CAST.Controllers
{

        [UseAntiForgeryTokenOnPostByDefault]
    public class AccountController : Controller
    {

        private AccountService accService;
        private Validator Validator { get; set; }
        private AdministrationService AdminService { get; set; }
        private AccountService AccountService { get; set; }
        private CustomerService customerService { get; set; }
        private ProcessService Service { get; set; }
        private string RetailClient_ID;
        private string Tech_support;
       
           

        /// <summary>
        /// Service
        /// </summary>
        public AccountController()
        {
            accService = (AccountService)Ioc.Get<AccountService>();
            Validator = (Validator)Ioc.Get<Validator>();
            AdminService = (AdministrationService)Ioc.Get<AdministrationService>();
            AccountService = (AccountService)Ioc.Get<AccountService>();
            customerService = (CustomerService)Ioc.Get<CustomerService>();
            Service = (ProcessService)Ioc.Get<ProcessService>();
            RetailClient_ID = System.Configuration.ConfigurationManager.AppSettings["RetailClientID"].ToString();
            Tech_support = System.Configuration.ConfigurationManager.AppSettings["Tech_support"].ToString();

        }

        [HttpGet]
        public ActionResult Enrolment(string EnrolmentCode, string returnUrl)
        {
            ViewBag.HideContact = true;
            ViewBag.HideFAQ = true;
            ViewBag.HideBack = true;
            ViewBag.HideHome = true;

            
            var enrolmentDetails = accService.GetEnrolment(EnrolmentCode);
            if (null != enrolmentDetails)
            {
                var accDetails = accService.GetAccountDetailsByCustomerId(enrolmentDetails.CustomerID);
                var newerEntrolment = accService.GetActiveEnrolmentByCustomerId(enrolmentDetails.CustomerID, enrolmentDetails.LinkType);

                if (null != enrolmentDetails)
                {


                    if (enrolmentDetails.ValidFlag && enrolmentDetails.LinkType == 0 && enrolmentDetails.EnroleId == newerEntrolment.EnroleId && accDetails.Email != null && accDetails.RetailClientId.ToString()== RetailClient_ID)
                    {
                        ViewBag.ReturnUrl = returnUrl ?? string.Empty;
                        return View((Object)EnrolmentCode);

                    }
                    else if (!enrolmentDetails.ValidFlag)
                    {
                        Log.File.ErrorFormat(string.Format("Date : {0}|  : SignIn from Enrolment : invalid link ", DateTime.Now));
                        return RedirectToAction("SignIn", "Account", new { returnUrl = string.Empty });
                      

                    }
                    else if (null != newerEntrolment && enrolmentDetails.EnroleId != newerEntrolment.EnroleId)
                    {
                        ViewBag.HideHome = true;
                        ViewBag.HideBack = true;
                        ViewBag.Customerservice = "Old Link";
                        ViewBag.CustomerserviceDetails = "This link doesn't work, Please use the latest one.";
                        return View("../BookNewService/CustomerService");

                    }

                    else if (enrolmentDetails.LinkType == 1)
                    {
                        return RedirectToAction("ChangePassword", "Account", new { EnrolmentCode = enrolmentDetails.EnrolmentCode.ToString() });

                    }
                    else if (null == accDetails.Email)
                    {
                        ViewBag.HideHome = true;
                        ViewBag.HideBack = true;
                        ViewBag.Customerservice = "Disabled Link";
                        ViewBag.CustomerserviceDetails = "Your link has been disabled. Please use newer link";
                        return View("../BookNewService/CustomerService");

                    }

                    else if (accDetails.RetailClientId.ToString() == RetailClient_ID)
                    {
                        ViewBag.HideHome = true;
                        ViewBag.HideBack = true;
                        ViewBag.Customerservice = "Invalid Link";
                        ViewBag.CustomerserviceDetails = string.Format("This is invalid link, Please contact our service team on {0} ",Tech_support);
                        return View("../BookNewService/CustomerService");
                    }

                    else
                    {
                        Log.File.ErrorFormat(string.Format("Date : {0}|  : SignIn from Enrolment : exisiting user ", DateTime.Now));
                        return RedirectToAction("SignIn", "Account", new { returnUrl =string.Empty });

                    }
                    
                }

                else
                {
                    ViewBag.Customerservice = "Wrong Link";
                    ViewBag.CustomerserviceDetails = "Your link is not valid, please contact our customer service";
                    return View("../BookNewService/CustomerService");

                }

            }
            else
            {
                ViewBag.Customerservice = "Wrong Link";
                ViewBag.CustomerserviceDetails = "Your link is not valid, please contact our customer service";
                return View("../BookNewService/CustomerService");

            }

        }

        [HttpPost]
        public ActionResult Enrolment(string Email, string PostCode, string returnUrl, string EnrolmentCode, string ClientCustRef)
        {
            string str = Request.Params["EnrolmentCode"];

            ViewBag.HideContact = true;
            ViewBag.HideFAQ = true;

            ViewBag.ReturnUrl = returnUrl ?? string.Empty;

            var enrolmentDetails = accService.GetEnrolment(EnrolmentCode);

            var customerDetails = customerService.GetCustomerInfo(enrolmentDetails.CustomerGuId);//customerService.GetCustomerByEmailPostCode(Email, PostCode);
           

                int enroleID = enrolmentDetails.EnroleId;

                if (string.IsNullOrEmpty(Email))
                {
                    ModelState.AddModelError("Email", "Please enter your email address.");
                }
                if (string.IsNullOrEmpty(PostCode))
                {
                    ModelState.AddModelError("PostCode", "Please enter your Postcode.");
                }


                //validation
                if (enrolmentDetails.Attempts < 4)
                {
                    if ((customerDetails.Email.ToUpper().Equals(Email)) && (customerDetails.Postcode.Replace(" ", "").Equals(PostCode.Replace(" ", "")) && (string.IsNullOrEmpty(ClientCustRef) || customerDetails.CLIENTCUSTREF.ToUpper().Equals(ClientCustRef.ToUpper()))))
                    {
                        return RedirectToAction("NewUser", "Account", new { Email = Email, PostCode = PostCode, ClientCustRef = ClientCustRef });
                    }

                    else if (!(customerDetails.Postcode.Replace(" ", "").Equals(PostCode.Replace(" ", ""))) && (customerDetails.Email.Equals(Email)))
                    {
                        accService.UpdateAttempts(enroleID);
                        ModelState.AddModelError("PostCode", "This details provided do not match our records, please try again.");
                        return View((Object)EnrolmentCode);

                    }
                    else if ((!(customerDetails.Email.ToUpper().Equals(Email))) && (customerDetails.Postcode.Replace(" ", "").Equals(PostCode.Replace(" ", ""))))
                    {
                        accService.UpdateAttempts(enroleID);
                        ModelState.AddModelError("PostCode", "This details provided do not match our records, please try again.");
                        return View((Object)EnrolmentCode);
                    }
                    else
                    {
                        accService.UpdateAttempts(enroleID);
                        ModelState.AddModelError("PostCode", "This details provided do not match our records, please try again.");
                        return View((Object)EnrolmentCode);
                    }
                }
                else
                {
                    return RedirectToAction("InputUserId", new { returnUrl = "NewEnrole" });
                }


            }
          
        public JsonResult IsUniqueMail(string Email, string urn)
        {


            var ShowWindow = false;
            bool isUniqueMail = true;


            try
            {
                var cusModel = customerService.GetCustomerByEmail(Email);
                if (cusModel.Count > 1 && string.IsNullOrEmpty(urn))
                {
                    isUniqueMail = true;

                }
                else
                {
                    isUniqueMail = false;
                }

            }

            catch (Exception e)
            {
                CAST.Logging.Log.File.Error(string.Format("error on Login : {0}", e.Message));
            }

            return Json(new { ShowWindow, isUniqueMail }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult IsUniqueAccount(string Email, string Postcode)
        {


            var ShowWindow = false;
            bool IsUniqueAccount = true;



            try
            {
                var accModel = customerService.GetCustomerByEmailPostCode(Email, Postcode);
                if (accModel.Count > 1)
                {
                    IsUniqueAccount = true;

                }
                else
                {
                    IsUniqueAccount = false;
                }
            }

            catch (Exception e)
            {
                CAST.Logging.Log.File.Error(string.Format("error on enrolment creation: {0}", e.Message));
            }

            return Json(new { ShowWindow, IsUniqueAccount, }, JsonRequestBehavior.AllowGet);
        }

        ////
        //// GET: /Account/
        [HttpGet]
        public ActionResult SignIn(string returnUrl)
        {
            ViewBag.HideBack = true;
            ViewBag.HideHome = true;
            string site = System.Configuration.ConfigurationManager.AppSettings["Brand"].ToString();
            string client_domain = System.Configuration.ConfigurationManager.AppSettings["client_domain"].ToString();

            ViewBag.Site = site;
            ViewBag.Domain = client_domain;
          


            if (string.IsNullOrEmpty(returnUrl))
                returnUrl = string.Empty;// Request.UrlReferrer.ToString();
            Log.File.InfoFormat(string.Format("Date : {0}| routing to SignIn ", DateTime.Now));
            var model = new CustomerAccount { ReturnUrl = returnUrl };
            return View(model);
        }

        [HttpPost]
        public ActionResult SignIn(CustomerAccount model)
        {
            ViewBag.HideBack = true;
            ViewBag.HideHome = true;
            customerService.ClearFromSession();

            Log.File.InfoFormat(string.Format("Date : {1}| Email : {0} ", model.Email,DateTime.Now));
            string site = System.Configuration.ConfigurationManager.AppSettings["Brand"].ToString();
            string client_domain = System.Configuration.ConfigurationManager.AppSettings["client_domain"].ToString();

            ViewBag.Site = site;
            ViewBag.Domain = client_domain;

            
            var accDetails = accService.GetAccountDetailsByEmail(model.Email);
            var accdetail = new CustomerAccount();
            if (accDetails.Count == 0)
            {
                ModelState.Clear();
                ModelState.AddModelError("Password", "The log in details do not match our records, please try again. After 5 incorrect attempts we will lock your account and send you email advising you what to do next. ");
                
                return View(new CustomerAccount());
            }
            if(model.ClientCustRef != null)
            {
                accdetail = accDetails.Where(x => x.ClientCustRef.ToLower().Trim() == model.ClientCustRef.ToLower().Trim()).FirstOrDefault();
            }
            else if (accDetails.Count == 1)
            {
                accdetail = accDetails.FirstOrDefault();
            }
           
            //validation
            if (string.IsNullOrEmpty(model.Email))
            {
                ModelState.Clear();
                ModelState.AddModelError("Email", "Please enter your email address.");
                return View(new CustomerAccount());
            }
            if (string.IsNullOrEmpty(model.Password))
            {
                ModelState.Clear();
                ModelState.AddModelError("Password", "Please enter your password.");
                return View(new CustomerAccount());
            }

            if (accdetail != null)
            {
                customerService.SessionInfo.CustomerGuId = accdetail.CustomerGuId;
                customerService.SessionInfo.CustomerID = accdetail.CustomerID;
                bool valid = BCryptHelper.CheckPassword(model.Password.Trim(), accdetail.Password);
                
            
                if ((!(valid) && !(string.IsNullOrEmpty(accdetail.Password))) || (!(RetailClient_ID.Equals(accdetail.RetailClientId.ToString()))))
                {
                    accService.UpdateTryCount(accdetail.Email);
                    ModelState.Clear();
                    ModelState.AddModelError("Password", "The log in details do not match our records, please try again. After 5 incorrect attempts we will lock your account and send you email advising you what to do next.");
                    int tryCount = accService.GetTryCount(accdetail.Email);
                    if (tryCount <= 4)
                    {

                        return View(new CustomerAccount());
                    }
                    else
                        return View("InputUserId", new CustomerAccount());

                }

            }
            else
            {

                ModelState.Clear();
                if (RetailClient_ID == "2")
                {
                    ModelState.AddModelError("Password", "The log in details do not match our records, please try again. After 5 incorrect attempts we will lock your account and send you email advising you what to do next. ");
                }
                else
                {
                    ModelState.AddModelError("Password", "The log in details do not match our records, please try again. After 5 incorrect attempts we will lock your account and send you email advising you what to do next. ");
                }
                return View(new CustomerAccount());
            }


       




            if (ModelState.IsValid)
            {
                accService.Authenticate(accdetail.CustomerID, (accdetail.Email));
                accService.SessionInfo.LastLoginDate = accdetail.LastLoginDate;

                accService.UpdateLastLogin(accdetail.CustomerID);

                int CustomerId = accService.GetCustomerId(accdetail.Email);

                return RedirectToAction("Details", "Customer");
            }

            return View(new CustomerAccount());
        }

        /// <summary>
        /// Sign out
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult SignOut()
        {
            // clear authentication cookie
            customerService.ClearFromSession();


            FormsAuthentication.SignOut();

           
            return Redirect(Url.Process(PredefinedProcess.SignIn));
        }

        /// <summary>
        /// Confidential info
        /// </summary>
        /// <returns>View</returns>
        [HttpGet]
        public ActionResult ChangePassword(string EnrolmentCode, string returnUrl)
        {
            ViewBag.HideBack = true;
            ViewBag.HideHome = true;
            if (accService.SessionInfo == null || string.IsNullOrEmpty(accService.SessionInfo.UserId))
            {
                ViewBag.ReturnUrl = returnUrl ?? string.Empty;
            }
           
            ViewBag.ReturnUrl = returnUrl ?? string.Empty;
            return View((Object)  EnrolmentCode );

        }

        /// <summary>
        /// Confidential info
        /// </summary>
        /// <returns>View</returns>
        [HttpPost]
        public ActionResult ChangePassword(string EnrolmentCode, string Password, string ConfirmPassword, string returnUrl)
        {
            string str = Request.Params["EnrolmentCode"];
            //validation
            if (string.IsNullOrEmpty(Password))
            {
                ModelState.AddModelError("Password", "Input password");
            }
            string userId = AccountService.SessionInfo.UserId;
            var enrolmentDetails = accService.GetEnrolment(str);
            var accDetails = accService.GetAccountDetailsByCustomerId(enrolmentDetails.CustomerID);
            if (accDetails != null)
            {
                bool validation = ValidatePassword(Password);
                if (!validation)
                {
                    accDetails.Password = string.Empty;
                    ModelState.AddModelError("Password", "your password should have minimum 8 character, and contains minimum one number.");
                }

                bool comparePassword = ComparePasswords(Password, ConfirmPassword);
                if (!comparePassword)
                {

                    ModelState.AddModelError("Password", "Both passwords don’t match, please try again.");
                }

                if (ModelState.IsValid)
                {
                    accService.UpdatePassword(enrolmentDetails.CustomerID, Password);
                    AccountService.UpdateCustomerEnrolment(enrolmentDetails.EnroleId);
                    accService.Authenticate(enrolmentDetails.CustomerID, accDetails.Email);
                    customerService.SessionInfo.CustomerID = enrolmentDetails.CustomerID;
                    customerService.SessionInfo.CustomerGuId = enrolmentDetails.CustomerGuId;
                    return RedirectToAction("Details", "Customer");
                }
            }
            else ModelState.AddModelError("Password", "This email hasn't registered.Please try again");

            ViewBag.UserId = accDetails.Email;
            ViewBag.ReturnUrl = returnUrl ?? string.Empty;
            return View(accDetails);

        }

        public ActionResult AccountWidget()
        {
          

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
        public ActionResult ConfidentialDetails(string userId, string returnUrl)
        {
            if (accService.SessionInfo == null || string.IsNullOrEmpty(accService.SessionInfo.UserId))
            {
                return Redirect("/");
            }
            var model = new AccountConfidentialInfo();
            ViewBag.UserId = accService.SessionInfo.UserId;
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
                accService.UpdateCofirmInfo(userId, model.Password, model.ReminderQuestion, model.ReminderAnswer);
                accService.Authenticate(0, userId);
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
           
            ViewBag.HideHome = true;
            ViewBag.HideBack = true;
           
            string site = System.Configuration.ConfigurationManager.AppSettings["Brand"].ToString();
            string client_domain = System.Configuration.ConfigurationManager.AppSettings["client_domain"].ToString();

            ViewBag.Site = site;
            ViewBag.Domain = client_domain;
       
            ViewBag.ReturnUrl = returnUrl ?? string.Empty;
            var model = new CustomerAccount { ReturnUrl = returnUrl };
            model.ReturnUrl = returnUrl;
            return View(model);

        }

        /// <summary>
        /// User id check
        /// </summary>
        /// <param name="userId">User id</param>
        /// <returns>Next step or error message</returns>
        [HttpPost]
        public ActionResult InputUserId(CustomerAccount model)
        {
            ViewBag.HideBack = true;
            ViewBag.ReturnUrl = model.ReturnUrl ?? string.Empty;
            CustomerAccount accDetail = new CustomerAccount();

            var accDetails = accService.GetCustomerByEmail(model.Email);

            
            string site = System.Configuration.ConfigurationManager.AppSettings["Brand"].ToString();
            string client_domain = System.Configuration.ConfigurationManager.AppSettings["client_domain"].ToString();

            ViewBag.Site = site;
            ViewBag.Domain = client_domain;
   

            if (accDetails.Count == 0)
            {

                ViewBag.HideHome = true;
                if (model.ReturnUrl != null)
                {
                    ViewBag.Customerservice = "Email Sent";
                    ViewBag.CustomerserviceDetails = "If you are an existing Service Guarantee customer you will receive link via email. ";
                }
                else
                {
                    ViewBag.Customerservice = "Password reset";                   
                    ViewBag.CustomerserviceDetails = "If you are an existing Service Guarantee customer you will receive link via email to reset your password. ";
                }
                ViewBag.CustomerserviceText = "Thank you.";
                
                return View("../BookNewService/CustomerService");

            }
            if (accDetails.Count > 1 && model.ClientCustRef != null)
            {
                try
                {
                    accDetail = accDetails.Where(x => x.ClientCustRef.ToUpper() == model.ClientCustRef.ToUpper()).FirstOrDefault();
                }
                catch (Exception ex)
                {
                    CAST.Logging.Log.File.Error(string.Format("error on Login : {0}", ex.Message));
                    accDetail = new CustomerAccount();
                }

            }
            else
            {
                accDetail = accDetails.FirstOrDefault();
            }

            

            //validation
            if (((accDetail == null || accDetail.CustomerID == 0) && accDetails.Count != 0))
            {
                 ViewBag.CustomerserviceText = "Thank you.";
                if (model.ReturnUrl != null)
                {
                    ViewBag.Customerservice = "Email Sent";
                    ViewBag.CustomerserviceDetails = "If you are an existing Service Guarantee customer you will receive link via email . ";
                }
                else
                {
                    ViewBag.Customerservice = "Password reset";
                    ViewBag.CustomerserviceDetails = "If you are an existing Service Guarantee customer you will receive link via email to reset your password. ";
                    
                }
               
             
                return View("../BookNewService/CustomerService");
               
            }

            if (ModelState.IsValid)
            {

                int linkType = 0;
                if (model.ReturnUrl != null)
                    linkType = 0;
                else
                    linkType = 1;

                if(linkType==1 && string.IsNullOrEmpty(accDetail.Password))
                {
                    ViewBag.Customerservice = "Password reset";
                    ViewBag.CustomerserviceText = "Thank you.";
                    ViewBag.CustomerserviceDetails = "If you are an existing Service Guarantee customer you will receive link via email to reset your password. ";
                    return View("../BookNewService/CustomerService");
               
                }

                if (linkType == 0 && !(string.IsNullOrEmpty(accDetail.Password)))
                {
                    ModelState.Clear();
                    ViewBag.Customerservice = "Email Sent";
                    ViewBag.CustomerserviceText = "Thank you.";
                    ModelState.AddModelError("Email", " You already have an active account. If you couldn't remember your password, please follow forgotten password link.");
                    CustomerAccount ca = new CustomerAccount();


                    return View("InputUserId", ca);
                }
                bool result = AccountService.CreateNewCustomerEnrole(accDetail.CustomerID, linkType);
                if (result)
                {

                    if (model.ReturnUrl == null)
                    {
                        ViewBag.Customerservice = "Password reset";
                        ViewBag.CustomerserviceText = "Thank you.";
                        ViewBag.CustomerserviceDetails = "If you are an existing Service Guarantee customer you will receive link via email to reset your password. ";
                        return View("../BookNewService/CustomerService");
                    
                    }
                    else
                    {
                        ViewBag.Customerservice = "Email Sent";
                        ViewBag.CustomerserviceText = "Thank you.";
                        ViewBag.CustomerserviceDetails = "If you are an existing Service Guarantee customer you will receive link via email to reset your password.";
                        return View("../BookNewService/CustomerService");
                    }
                }
                else
                {
                    ViewBag.Customerservice = "Forgotten Password";
                    ViewBag.CustomerserviceDetails = "Oops! try again please!.";
                    return View("../BookNewService/CustomerService");
                }
            }
          
            return View(model);
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
            var postCode = accService.GetPostcode(Convert.ToInt32(userId));
            bool validatePostcode = (postCode.Replace(" ", "")).Equals(answer.Replace(" ", ""));
            if (string.IsNullOrEmpty(answer) || !validatePostcode)
            {
                ModelState.Clear();
                ModelState.AddModelError("Postcode", "Wrong postcode.");
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

        [HttpGet]
        public ActionResult NewUser(string Email, string PostCode, string ClientCustRef)
        {
            ViewBag.HideContact = true;
            ViewBag.HideFAQ = true;
            ViewBag.HideBack = true;
            ViewBag.HideHome = true;
            customerService.ClearFromSession();
            AccountService.ClearFromSession();
            var model = new CustomerAccount();
            var cusmodel = customerService.GetCustomerDataByEmailCustURN(Email, ClientCustRef);
            if (null != cusmodel)
            {
                model.InjectFrom(cusmodel);
                customerService.SessionInfo.CustomerID = model.CustomerID;
                customerService.SessionInfo.CustomerGuId = model.CustomerGuId;
                AccountService.SessionInfo.EnroleId = model.EnroleId;
                return View(model);
            }
            else
            {
                ModelState.Clear();
                ModelState.AddModelError("Postcode", "The details do not match our records, please try again. After 5 incorrect attempts we will disable your link. ");
                return View(model);
            }
        }

        /// <summary>
        /// Try to save colleague
        /// </summary>
        /// <param name="model">fill form</param>
        /// <returns>success  - show view with message
        /// fault    - previous view </returns>
        [HttpPost]
        public ActionResult NewUser(CustomerAccount model)
        {
            ViewBag.HideContact = true;
            ViewBag.HideFAQ = true;
            ViewBag.HideBack = true;
            ViewBag.HideHome = true;
            ModelState.Clear();
            ViewBag.Success = false;
            var accountDetails = new AccountService();
            model.LastLoginDate = new DateTime(1900, 01, 01);
            model.Email = model.Email;
            //validation

            if (string.IsNullOrEmpty(model.Password) || string.IsNullOrEmpty(model.Email))
            {
                ModelState.Clear();
                ModelState.AddModelError("Email", "Empty User or Password.");
                return View(model);
            }
            Log.File.InfoFormat(string.Format("Date : {1}| Email : {0} ", model.Email, DateTime.Now));
            var user = new CustomerAccount();
            user = accountDetails.GetCustomerAccountDetails(model.Email, model.PostCode, model.ClientCustRef);

            //

            if (user != null)
            {
                ModelState.Clear();
                ModelState.AddModelError("Password", "User already exist.");
            }

            bool validation = ValidatePassword(model.Password);
            if (!validation)
            {
                ModelState.Clear();
                ModelState.AddModelError("ConfirmPassword", "Your password must be at least 8 characters in length and contain at least one number, please try again. ");
            }

            bool comparePassword = ComparePasswords(model.Password, model.ConfirmPassword);
            if (!comparePassword)
            {
                ModelState.Clear();
                ModelState.AddModelError("Password", "Both passwords don’t match, please try again.");
            }

            if (ModelState.IsValid)
            {
               
                // user levels
                var accDetails = new CustomerAccount();
                accDetails.InjectFrom(model);
                accDetails.Password = model.Password;
                AccountService.AddNewCustomerAccount(accDetails);
                AccountService.UpdateCustomerEnrolment(AccountService.SessionInfo.EnroleId);
                AccountService.InsertUserTryCount(model.Email);
               
                accService.Authenticate(customerService.SessionInfo.CustomerID , (accDetails.Email));
              
                return RedirectToAction("Details", "Customer");
            }
            else
            {
                Log.File.ErrorFormat(string.Format("Date : {1}| Email : {0} : Returning to signin page from NewUser ", model.Email, DateTime.Now));
                return View(model);
            }
        }

        private bool ComparePasswords(string password, string confirmPassword)
        {
            if (String.Compare(password, confirmPassword) == 0)
            {
                return true;
            }
            else
                return false;
        }

        private bool ValidatePassword(string password)
        {
            string pattern = @"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{8,}$";
            Regex rgx = new Regex(pattern, RegexOptions.IgnoreCase);
            MatchCollection matches = rgx.Matches(password);
            if (matches.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
            // return rgx.IsMatch();
        }



        /// <summary>
        /// Page if access denied
        /// </summary>
        /// <param name="role">Role name for message</param>
        /// <returns></returns>
        public ActionResult AccessDenied(string role = "Administrator")
        {
            Log.File.ErrorFormat(string.Format("Date : {0}|  : AccessDenied  ", DateTime.Now));
            return View(model: role);
        }
    }
}
