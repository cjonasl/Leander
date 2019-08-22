using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using CAST.Configuration;
using CAST.Infrastructure;
using CAST.Logging;
using CAST.Models.Account;
using CAST.ViewModels.Account;
using CAST.Properties;
using CAST.Repositories;
using BCrypt.Net;
using DevOne.Security.Cryptography.BCrypt;



namespace CAST.Services
{
    public class AccountService :Service, IService
    {
        private AccountRepository Repository = new AccountRepository();

        /// <summary>
        /// Session model
        /// </summary>
        public SessionModel SessionInfo
        {
            get { return Session.Load(new SessionModel()); }
        }


        /// <summary>
        /// Account details
        /// </summary>
        /// <param name="userId">User id</param>
        /// <returns></returns>
        public AccountDetailsModel GetAccountDetails(string userId)
        {
            return Repository.GetAccountDetails(userId);
        }
        public AccountDetailsModel GetAccountDetailsForCustomer(int CustomerId)
        {
            return Repository.GetAccountDetailsForCustomer(CustomerId);
        }
        //GetCustomerAccountDetailsGetAccountDetailsForCustomer
        public CustomerAccount GetCustomerAccountDetails(string Email,string postcode,string customerURN)
        {
            return Repository.GetCustomerAccountDetails(Email,postcode,customerURN);
        }
        public List<CustomerAccount> GetAccountDetailsByEmailUrn(string Email, string customerURN)
        {
            return Repository.GetAccountDetailsByEmailUrn(Email, customerURN);//, postcode, customerURN);
        }

        public List<CustomerAccount> GetAccountDetailsByEmail(string Email, int RetailClientId)
        {
            return Repository.GetAccountDetailsByEmail(Email, RetailClientId);//, postcode, customerURN);
        }
        //CustomerAccountDetailsByEmail
        public CustomerAccount GetAccountDetailsByCustomerId(int customerId)
        {
            return Repository.GetAccountDetailsByCustomerId(customerId);
        }
        public List<CustomerAccount> GetCustomerByEmail(string Email)
        {
            return Repository.GetCustomerByEmail(Email);//, postcode, customerURN);
        }
        //public AccountDetailsModel GetAccountDetailsByEnroleId(Int32 enroleID)
        //{
        //    return Repository.GetAccountDetailsByEnroleId(enroleID);
        //}
        //public AccountDetailsModel GetCustomerAccountByEnroleId(Int32 customerId)
        //{
        //    return Repository.GetAccountDetailsByCustomerId(customerId);
        //}
        

        public EntrolmentViewModel GetEnrolment(string entrolmentCode)
        {
            return Repository.GetEnrolmentDetails(entrolmentCode);
        }

        public EntrolmentViewModel GetActiveEnrolmentByCustomerId(int customerId, int linkType)
        {
            return Repository.GetActiveEnrolmentByCustomerId(customerId, linkType);
        }

        //Retrieve_ActiveEnrolmentByCustomerId
        public EntrolmentViewModel GetEnrolmentById(int entrolmentId)
        {
            return Repository.GetEnrolmentDetailsById(entrolmentId);
        }
        public string GetPostcode(int userId)
        {
            return Repository.GetPostcode(userId);
        }
        public int GetEntroleId(int customerId)
        {
            return Repository.GetEnrolmentID(customerId);
        }

        /// <summary>
        /// Run auto diagnostic or not
        /// </summary>
        //public bool IsDiagnosted
        //{
        //    //get
        //    //{
        //    //    var result = Cookies.Load(CookiesKeys.IsAutoDiagnosted) ?? "False";
        //    //    return System.Convert.ToBoolean(result);
        //    //}
        //    //set
        //    //{
        //    //    Cookies.UpdateFrom(value.ToString(), CookiesKeys.IsAutoDiagnosted);
        //    //}
        //}

        /// <summary>
        /// Authenticate user
        /// </summary>
        /// <param name="userId"></param>
        public void Authenticate(int  CustomerId, string userId)
        {
            //if (CookiesStoredInfo.StoreId == null)
            //{
            var accDetails = GetAccountDetailsForCustomer(CustomerId);
            //    CookiesStoredInfo = new CookiesStoredInfo
            //    {
            //        IsCallCenter = !accDetails.ClientPriorityBooking,
            //        IsDiagnosted = true,
            //        StoreId = accDetails.UserStoreId.ToString(),
            //       SuperAdmin =accDetails.AccessLevel==0,
            //        StoreName = accDetails.UserStoreName
            //    };
            //}

            FormsAuthentication.SetAuthCookie(userId, true);
            // Add log record to database
            var browser = HttpContext.Current.Request.Browser;
            Log.Database.SignIn.Add(userId, String.Format("{0} v{1}", browser.Browser, browser["version"]));
        }

        /// <summary>
        /// Cookies account info
        /// </summary>
        //public CookiesStoredInfo CookiesStoredInfo
        //{
        //    get
        //    {
        //        var result = new CookiesStoredInfo();
        //        result.IsDiagnosted = Convert.ToBoolean(Cookies.Load(CookiesKeys.IsAutoDiagnosted));
        //        result.IsCallCenter = Convert.ToBoolean(Cookies.Load(CookiesKeys.CallCenter));
        //        result.SuperAdmin = Convert.ToBoolean(Cookies.Load(CookiesKeys.SuperAdmin));
        //        result.StoreId = Cookies.Load(CookiesKeys.StoreNumber);
        //        result.StoreName = Cookies.Load(CookiesKeys.StoreName);
        //        return result;
        //    }
        //    set { 
        //        Cookies.UpdateFrom(value.IsDiagnosted.ToString(), CookiesKeys.IsAutoDiagnosted);
        //        Cookies.UpdateFrom(value.IsCallCenter.ToString(), CookiesKeys.CallCenter);
        //        Cookies.UpdateFrom(value.StoreId, CookiesKeys.StoreNumber);
        //        Cookies.UpdateFrom(value.SuperAdmin.ToString(), CookiesKeys.SuperAdmin);
        //        Cookies.UpdateFrom(value.StoreName, CookiesKeys.StoreName);
        //    }
        //}

        /// <summary>
        /// Update password
        /// </summary>
        /// <param name="userId">User id</param>
        /// <param name="newPassword">New password</param>
        public void UpdatePassword(int CustomerId, string newPassword)
        {

            string encryptedPassword = BCryptHelper.HashPassword(newPassword, BCryptHelper.GenerateSalt(6));
                //Utils.Encryption.HashString(newPassword.Trim() + Constants.UserSalt);
                
                // Utils.Encryption.HashString(newPassword + Constants.UserSalt);
            Repository.UpdatePassword(CustomerId, encryptedPassword);
        }

        /// <summary>
        /// Save confidential info
        /// </summary>
        /// <returns></returns>
        public bool UpdateCofirmInfo(string userId, string password, string question, string answer)
        {
            
            try
            {
                Repository.UpdateConfidentialInfo(userId, password, question, answer);
                return true;
            }
            catch (Exception e)
            {
                Log.File.Info(Msg.GenerateLogMsg(string.Format("Error while confidential info was saving for user {0}. Error:{1}", userId, e.Message)));
                return false;
            }
        }

        /// <summary>
        /// Is user in role
        /// </summary>
        /// <param name="userId">User id</param>
        /// <param name="role">Role name</param>
        /// <returns></returns>
        public bool IsUserInRole(string userId, string role)
        {
            var userDetails = GetAccountDetails(userId);
            //var roles = role.Split(',');
            //var test = Enum.IsDefined(typeof(UserRoles.Levels), roles);
            //var test2 = Enum.IsDefined(typeof(UserRoles.Levels), roles[0]);
            var roleLevel = (int)Enum.Parse(typeof(UserRoles.Levels), role);
           return userDetails.UserLevel == roleLevel;
            //return false;
        }

        /// <summary>
        /// Get accounts list by pages
        /// </summary>
        /// <param name="page">Page number</param>
        /// <returns></returns>
        //public List<AccountDetailsModel> Accounts(int page)
        //{
        //    return Repository.FindUsers(StoreId, page, Settings.Default.UserSearchPageSize);
        //}

        /// <summary>
        /// Delete account
        /// </summary>
        /// <param name="userId">User id</param>
        /// <returns></returns>
        public bool DeleteAccount(string userId)
        {
            if (!string.IsNullOrEmpty(userId))
            {
                Repository.DeleteAccount(userId);
            }
            return true;
        }

        /// <summary>
        /// Add new usr or update exist
        /// </summary>
        /// <param name="model">Model data</param>
        /// <param name="newUserId">New user id</param>
        /// <returns></returns>
        public bool AddOrUpdateUser(AccountDetailsModel model, string newUserId)
        {
             return Repository.AddOrUpdateUser(model, newUserId);
        }
        public bool AddNewCustomerAccount(CustomerAccount model)
        {
            string encryptedPassword = BCryptHelper.HashPassword(model.Password, BCryptHelper.GenerateSalt(6));
            model.Password = encryptedPassword;
            return Repository.AddNewCustomerAccount(model);
        }
        public void ClearFromSession()
        {
            Session.Clear(SessionInfo);
        }

        public bool UpdateCustomerEnrolment(int EnroleId)
        {
            return Repository.UpdateCustomerEnrolment(EnroleId);
        }
        public void UpdateAttempts(int enroleID)
        {
            Repository.UpdateAttempts(enroleID);
        }

        public Int32 GetCustomerId(string email)
        {
            return Repository.GetCustomerId(email);

        }

        public  bool CreateNewCustomerEnrole(Int32 customerID,int linkType)
        {

            return Repository.InsertCustomerEnrolment(customerID, linkType);
           
        }



        public  void DisableEnrolementLink(int customerId, int enroleId)
        {
            Repository.DisableEnrolementLink(customerId, enroleId);
           
        }

        public void UpdateTryCount(string UserId)
        {
            Repository.UpdateTryCount(UserId);
        }
        //ClearTryCount

        public void ClearTryCount(string UserId)
        {
            Repository.ClearTryCount(UserId);
        }
        public  void InsertUserTryCount(string UserId)
        {
            Repository.InsertUserTryCount(UserId);
        }

        public int GetTryCount(string UserId)
        {
           return Repository.GetTryCount(UserId);
        }



        public void UpdateLastLogin(int customerId)
        {
            Repository.UpdateLastLogin(customerId);
        }
    }
}