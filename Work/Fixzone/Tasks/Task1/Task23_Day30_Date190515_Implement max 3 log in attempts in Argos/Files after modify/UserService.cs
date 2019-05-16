using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using CAST.Infrastructure;
using CAST.Logging;
using CAST.Repositories;
using CAST.Sessions;
using CAST.User;
using CAST.ViewModels.User;


namespace CAST.Services
{
    public class UserService
    {

        /// <summary>
        /// Product data access object
        /// </summary>
        private readonly UserRepository _reporsitory;

        /// <summary>
        /// Data context
        /// </summary>
        private DataContext _dataContext;

        /// <summary>
        /// Represents _bookRepairState of bookRepair processes
        /// </summary>
        private UserStateHolder _userStateHolder;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="data"></param>
        public UserService(DataContext data)
        {
            _dataContext = data;
            _reporsitory = new UserRepository(data);
            _userStateHolder = new UserStateHolder();
        }

        #region GET INFO
        
        /// <summary>
        /// Get store id
        /// </summary>
        /// <returns></returns>
        public string GetUserStoreId()
        {
            var user = _userStateHolder.Load();
            return user.UserId;
        }

        /// <summary>
        /// Check is user id exist
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool IsUserIdExist(string userId)
        {
            return _reporsitory.IsUserIdExist(userId);
        }

        /// <summary>
        /// Get user id 
        /// </summary>
        /// <returns></returns>
        public string GetUserId()
        {
            var user = _userStateHolder.Load();
            return user.UserId ?? "";
        }

        /// <summary>
        /// Get first time user id 
        /// </summary>
        /// <returns></returns>
        public string GetFirstTimeUserId()
        {
            var user = _userStateHolder.Load();
            return user.FirstTimeUserId;
        }

        /// <summary>
        /// Get URL for return from sign in page
        /// </summary>
        /// <returns></returns>
        public string GetUrlForBack()
        {
            var user = _userStateHolder.Load();
            return user.UrlForReturn;
        }

        /// <summary>
        /// return flag is sign in automatically
        /// </summary>
        /// <returns></returns>
        public bool IsAutoSignIn()
        {
            var user = _userStateHolder.Load();
            return user.IsAutoSignIn ?? false;
        }

        /// <summary>
        /// Get is call center or not
        /// </summary>
        /// <returns></returns>
        public bool IsCallCenter()
        {
            var store = new StoreState();
            return !store.ClientPriorityBooking;
        }

        public IList<SelectListItem> GetUserLevelsOfAccess()
        {
            return _reporsitory.GetUserLevelsOfAccess();
        }

        /// <summary>
        /// Get user full name
        /// </summary>
        /// <returns></returns>
        public string GetUserFullName()
        {
            var user = _userStateHolder.Load();
            return user.UserFullName ?? "";
        }

        /// <summary>
        /// Check user date of birth
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool CheckUserDateOfBirth(string userId, User_DateOfBirthModel model)
        {
            return _reporsitory.CheckUserDateOfBirth(userId, model);
        }

        /// <summary>
        /// Check if user with Id and pass exists in the base
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="password"></param>
        /// <returns>Model where the field IsUserExist shows if this user exists in the database</returns>
        public User_DetailsModel GetUserInfo(string userId, string password)
        {
            var _store = new StoreService(_dataContext);
            var info = _reporsitory.UserInfo(userId, password);
            info.RunAutoDiagnostic = Convert.ToInt32(!_store.IsAutoDiagnosted);
            return info;
        }


        public bool GetUserInfo(string userId)
        {
            var _store = new StoreService(_dataContext);
            var info = _reporsitory.UserInfo(userId);
            
            return info;
        }

        /// <summary>
        /// Check whether the user with id has an empty pass (first time entering to the system)
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool IsUserPasswordEmpty(string userId)
        {
            return _reporsitory.IsUserPasswordEmpty(userId);
        }

        /// <summary>
        /// Get the confirm answer for the user identity 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public string GetConfirmAnswer(string userId)
        {
            return _reporsitory.GetConfirmAnswer(userId);
        }

        /// <summary>
        /// Check if user give the correct answer on the user identity question
        /// </summary>
        /// <param name="reminderUserAnswer">Answer</param>
        /// <param name="userId">user id</param>
        /// <returns></returns>
        public bool IsRightAnswerOnQuestion(string reminderUserAnswer, string userId)
        {
            return _reporsitory.IsRightAnswerOnQuestion(reminderUserAnswer, userId);
        }

        /// <summary>
        /// Try to save the new unique user password
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="newPassword"></param>
        /// <returns>true if the user used this pass before</returns>
        public bool SaveNewPassword(string userId, string newPassword)
        {
            return _reporsitory.SaveNewPassword(userId, newPassword);
        }

        //Allow user to change their store
        public void UpdateUserStore(string userId, string ClientId)
        {
             _reporsitory.UpdateUserStore(userId, ClientId);
        }
        
        /// <summary>
        /// get details of user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public User_DetailsModel GetUserDetails(string userId)
        {
            return _reporsitory.GetUserDetails(userId);
        }


        #endregion

        #region Set Info
        /// <summary>
        /// Set first time id
        /// </summary>
        /// <param name="userId"></param>
        public void SetFirstTimeUserId(string userId)
        {
            var user = _userStateHolder.Load();
            user.FirstTimeUserId = userId;
            _userStateHolder.UpdateFrom(user);
        }

        /// <summary>
        /// Set URL for return from sign in page
        /// </summary>
        /// <returns></returns>
        public void SetUrlForBack(string url)
        {
            var user = _userStateHolder.Load();
            user.UrlForReturn = url;
            _userStateHolder.UpdateFrom(user);
        }

        /// <summary>
        /// Set flag is sign in automatically
        /// </summary>
        /// <returns></returns>
        public void IsAutoSignIn(bool auto)
        {
            var user = _userStateHolder.Load();
            user.IsAutoSignIn = auto;
            _userStateHolder.UpdateFrom(user);
        }

        /// <summary>
        /// Set user id
        /// </summary>
        /// <param name="id"></param>
        public void SetUserId(string id)
        {
            var user = _userStateHolder.Load();
            user.UserId = id;
            _userStateHolder.UpdateFrom(user);
        }

        /// <summary>
        /// Set user full name
        /// </summary>
        /// <param name="name"></param>
        public void SetUserName(string name)
        {
            var user = _userStateHolder.Load();
            user.UserFullName = name;
            _userStateHolder.UpdateFrom(user);
        }

        //finding 24/7 call center
        private void SetGroup(int groupId)
        {

            var user = _userStateHolder.Load();
            user.IsOffshoreCallCenter = (groupId==8);
            _userStateHolder.UpdateFrom(user);
        }

        /// <summary>
        /// Set store id
        /// </summary>
        /// <param name="id"></param>
        public void SetStoreId(int id)
        {
            var store = new StoreState();
            string name = string.Empty;
            if (store.StoreInfo != null) name = store.StoreInfo.StoreName;
            var info = new StoreInfo { StoreNumber = id, StoreName = name };
            store.StoreInfo = info;
        }

        /// <summary>
        /// Set store name
        /// </summary>
        /// <param name="name"></param>
        public void SetStoreName(string name)
        {
            var store = new StoreState();
            int? num = null;
            if (store.StoreInfo != null) num = store.StoreInfo.StoreNumber;
            var info = new StoreInfo { StoreName = name, StoreNumber = num };
            store.StoreInfo = info;
        }

        /// <summary>
        /// Set flag is call center or not
        /// </summary>
        /// <param name="flag"></param>
        public void IsCallCenter(bool flag)
        {
            var store = new StoreState();
            store.ClientPriorityBooking = flag;
         
        }

        public void IsOffshoreCallCenter( int GroupID)
        {
            var store = new StoreState();
            
            store.IsOffShoreCallcenter = (GroupID == 8);
        }
        
        public void SaveUserDetails(User_ConfidentialInfoModel model)
        {
            _reporsitory.SaveUserDetails(model);
        }

        public void SaveUserDetails(User_AccountInfoModel model)
        {
            _reporsitory.SaveUserDetails(model);
        }
        #endregion

        /// <summary>
        /// Set auth info
        /// </summary>
        /// <param name="model"></param>
        public void SetAuthInfo(User_DetailsModel model)
        {
            var _storeService = new StoreService(_dataContext);

            // This is made for clean back url from session and flag
            var urlForback = GetUrlForBack() ?? "/";
            bool AutoSignIn = this.IsAutoSignIn();

            // Clear from Session
            SetUrlForBack(null);
            IsAutoSignIn(false);
            SetUserId(model.UserId);
            SetUserName(model.UserName);
            SetGroup(model.GroupID);

            // If no store indfo then set store in cookies
            if (model.UserStoreID.HasValue && (!_storeService.IsStoreInfoExist()))
            {
                _storeService.SetStoreInfo(model.UserStoreID.Value, model.UserStoreName);
                IsCallCenter(model.ClientPriorityBooking);
            }
            if (model.UserStoreID.HasValue && (_storeService.IsStoreInfoExist()))
                 IsOffshoreCallCenter( model.GroupID);
            FormsAuthentication.SetAuthCookie(model.UserId, createPersistentCookie: false);

            // Add log record to database
            var log = new Log(_dataContext);
            var browser = HttpContext.Current.Request.Browser;
            log.Database.SignIn.Add(String.Format("{0} v{1}", browser.Browser, browser["version"]));
        }

        

        /// <summary>
        /// Clear info from session
        /// </summary>
        public void ClearInfoFromSession()
        {
            _userStateHolder.Clear();
        }

        public void DisableUser(string userId)
        {
            _reporsitory.DisableUser(userId);
        }

        internal User_AccountInfoModel GetUserAccountInfo(string userId)
        {
          return  _reporsitory.GetUserDetailsForUpdate( userId);
        }
    }
}