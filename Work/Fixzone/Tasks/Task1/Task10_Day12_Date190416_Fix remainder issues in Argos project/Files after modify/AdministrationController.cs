using System;
using System.Web.Mvc;
using CAST.Models.Administration;
using CAST.Properties;
using CAST.Roles;
using CAST.Services;
using CAST.Process;

namespace CAST.Controllers
{
    /// <summary>
    /// Administration page
    /// </summary>
    [AdminRole]
    public class AdministrationController : DataController
    {
        
        /// <summary>
        /// User account state
        /// </summary>
        private readonly UserService _userService;

        /// <summary>
        /// Store state
        /// </summary>
        private readonly StoreService _storeService;

        /// <summary>
        /// Admin service
        /// </summary>
        private readonly AdministrationService _adminService;
        
        public AdministrationController()
        {
            _userService = new UserService(Data);
            _storeService = new StoreService(Data);
            _adminService = new AdministrationService(Data);
        }


        /// <summary>
        /// Show users list
        /// </summary>
        /// <param name="pageNum"> The page Num. </param>
        /// <returns> List of users </returns>
  
        public ActionResult UserList(int pageNum = 1)
        {
            var userId = _userService.GetUserId();

            // user administration should be available only to logged-in users
            if (string.IsNullOrEmpty(userId))
            {
                return Redirect(Url.Process(PredefinedProcess.SignIn));
            }

            var usersListModel = _adminService.GetUsersList(_storeService.GetStoreId(), pageNum, Settings.Default.UserSearchPageSize);
           ViewBag.levels = Settings.Default.LevelsOfAccess;

            ViewBag.StartLevelValue = Settings.Default.StartValueForAccessLevel;
            return View(usersListModel);
        }

        #region Get User Details
        /// <summary>
        /// Save edeting user id  in session
        /// </summary>
        /// <param name="id">edeting user ID</param>
        /// <returns>redirect first step</returns>
       public ActionResult GoToUserDetails(string id)
        {
           // SAVE USER ID since admin can change unique ID;
            
            _adminService.SetUserIdForEditing(id);
            return Redirect(Url.Process(PredefinedProcess.UserDetails));
        }


        /// <summary>
        /// - Shows user information
        /// </summary>
        /// <returns>View user details</returns>
       
        public ActionResult UserInformation()
        {
            
            // get user info
            var model = _adminService.GetUserDetailsForUpdate(_adminService.GetUserIdForEditing());
            if (model == null) return Redirect(Url.ProcessPreviousStep());
            model.LevelsOfAccess = _userService.GetUserLevelsOfAccess();
            ViewBag.IsCallCenter = _storeService.IsCallCenter();
            return View(model);
        }

        /// <summary>
        /// Save user
        /// </summary>
        /// <param name="userDetails">User details from form</param>
        /// <returns>result of saving</returns>
        [HttpPost]
    
        public ActionResult UserInformation(AdministrationUserModel model)
        {
            
            ViewBag.IsEditMode = true;
            model.LevelsOfAccess = _userService.GetUserLevelsOfAccess();
            ViewBag.IsCallCenter = _storeService.IsCallCenter();
            // Check if user want to change his id
            if (model.UserId != _adminService.GetUserIdForEditing())
            {
                // check if new userId is not unique
                if (_userService.IsUserIdExist(model.UserId))
                {
                    ModelState.AddModelError("UserId", "That ID already exists. ");
                    return View(model);
                }
            }

            //// Check date
            if (ModelState.IsValid && _storeService.IsCallCenter())
            {
                try
                {
                    var date = String.Format("{0:00}/{1:00}/{2:0000}", model.Day, model.Month, model.Year);
                    DateTime.ParseExact(date, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                }
                catch (Exception e)
                {
                    ModelState.AddModelError("Day", "Wrong date.");
                    ModelState.AddModelError("Month", " ");
                    ModelState.AddModelError("Year", " ");
                }
            }

            // If model is valid, then try update user info
            if(ModelState.IsValid)
            {
                try
                {
                   if(_storeService.IsCallCenter()) _adminService.InsertOrUpdateCallCenterUser(model, _adminService.GetUserIdForEditing());
                   else  _adminService.InsertOrUpdateUser(model, _adminService.GetUserIdForEditing());
                    ViewBag.resultMsg = "<div class=\"success\">Saved Successfully</div>";
                    ViewBag.IsEditMode = false;
                    _adminService.SetUserIdForEditing(model.UserId);
                }
                catch (Exception ex)
                {
                    ViewBag.resultMsg = "<div class=\"error\">Save was not completed</div>";
                }
            }
            return View(model);
        }
        #endregion

        #region Add New Colleague


        /// <summary>
        /// - first step - show form for new colleague
        /// </summary>
        /// <param name="model">model for showing</param>
        /// <returns>View form for filling</returns>
     
        public ActionResult AddColleague()
        {
          // set UserLevel to Store Coleague 
            var model = new AdministrationUserModel();
            model.UserLevel = Settings.Default.LevelsOfAccess.IndexOf("2 - Store colleague") + Settings.Default.StartValueForAccessLevel;
            model.LevelsOfAccess = _userService.GetUserLevelsOfAccess();
            ViewBag.IsCallCenter = _storeService.IsCallCenter();
            return View(model);
        }

        /// <summary>
        /// Try to save colleague
        /// </summary>
        /// <param name="model">fill form</param>
        /// <returns>success  - show view with message
        /// fault    - previous view </returns>
        [HttpPost]

        public ActionResult AddColleague(AdministrationUserModel model)
        {
            bool isUserSaved = false;

            model.LevelsOfAccess = _userService.GetUserLevelsOfAccess();

            // Check is userId is unique
            if (!String.IsNullOrEmpty(model.UserId))
            {
                if (model.UserId.Length < 26)
                {
                    if (_userService.IsUserIdExist(model.UserId))
                        ModelState.AddModelError("UserId", "That ID already exists. ");
                }
                else
                {
                    ModelState.AddModelError("UserId", "ID should not be more than 25");
                }
            }

            model.StoreNumber = _storeService.GetStoreId();
            model.LevelsOfAccess = _userService.GetUserLevelsOfAccess();

            
            if (ModelState.IsValid)
            {
                //Check date of birth
                bool? errorVariableIsYear;
                string errorMessage = CAST.Validation.DateOfBirthValidation.Check(model.Year, model.Month, model.Day, out errorVariableIsYear);

                if (!string.IsNullOrEmpty(errorMessage))
                {
                    if (errorVariableIsYear.Value)
                        ModelState.AddModelError("Year", errorMessage);
                    else
                        ModelState.AddModelError("Day", errorMessage);
                }
                else // Create new user
                {
                    if (_storeService.IsCallCenter())
                        isUserSaved = _adminService.AddNewContactCenterColleague(model, _userService.GetUserId());
                    else
                        isUserSaved = _adminService.AddNewColleague(model, _userService.GetUserId());
                }
            }
            ViewBag.IsCallCenter = _storeService.IsCallCenter();
            ViewBag.IsSavedSuccess = isUserSaved;
            return View(model);
        }
        #endregion

        #region Delete User

        /// <summary>
        /// Deleting user
        /// </summary>
        /// <returns>Page with result</returns>
        public bool DeleteUser(string userId)
        {
            bool success = false;
           if (!string.IsNullOrEmpty(userId))
                {
                    // user id get from session
                    try
                    {
                        _adminService.DeleteUser(userId);
                        success = true;
                    }
                    catch (Exception)
                    {
                        success = false;
                    }
                }
    return success;
        }

        #endregion
    }
}
