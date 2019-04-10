using System.Web.Mvc;
using CAST.Infrastructure;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using CAST.ViewModels.User;
using Dapper;
using CAST.Models.User;

namespace CAST.Repositories
{
    public class UserRepository
    {
        private DataContext _dataContext;

        public UserRepository(DataContext data)
        {
            _dataContext = data;
        }

        #region GET INFO
        /// <summary>
        /// Return true, if user exist and false - if not.
        /// </summary>
        /// <param name="userId">Employee number</param>
        /// <param name="password">Employee password</param>
        /// <returns>True or false</returns>
        public User_DetailsModel UserInfo(string UserId, string Password)
        {
            var model = _dataContext.Connection.Query<User_DetailsModel>("SignIn",
                                                                   new { UserId, Password },
                                                                   commandType: CommandType.StoredProcedure).FirstOrDefault();
            //model.Enabled = true;
            return model;
        }


        public bool UserInfo(string UserId)
        {
            bool StoreUseridentification = _dataContext.Connection.Query<bool>("StoreUseridentification",
                                                                   new { UserId },
                                                                   commandType: CommandType.StoredProcedure).FirstOrDefault();
            //model.Enabled = true;
            return StoreUseridentification;
        }
        /// <summary>
        /// Chaeck date of birth 
        /// </summary>
        /// <param name="firstTimeUserId">user id for first time sign in</param>
        /// <param name="model">Info model</param>
        /// <returns></returns>
        public bool CheckUserDateOfBirth(string firstTimeUserId, User_DateOfBirthModel model)
        {
            
            DateTime userBirthDateFormated = new DateTime(model.Year, model.Month, model.Day);


            bool result = _dataContext.Connection.Query<bool>("CheckDateOfBirthByID",
                                                            new { userId = firstTimeUserId, dateOfBirth = userBirthDateFormated },
                                                            commandType: CommandType.StoredProcedure).FirstOrDefault();
            return result;
        }

        /// <summary>
        /// Is user exist
        /// </summary>
        /// <param name="userId">User id</param>
        /// <returns></returns>
        public bool IsUserIdExist(string userId)
        {
            bool userExist = _dataContext.Connection.Query<bool>("UserIdExist",
                                                                  new {userId },
                                                                  commandType: CommandType.StoredProcedure).FirstOrDefault();
            return userExist;
        }

        /// <summary>
        /// Return is password empty or not
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool IsUserPasswordEmpty(string userId)
        {
            bool passwordEmpty = _dataContext.Connection.Query<bool>("UserPasswordEmpty",
                                                                  new { userId },
                                                                  commandType: CommandType.StoredProcedure).FirstOrDefault();
            return passwordEmpty;
        }

        /// <summary>
        /// Get confirm answer for user
        /// </summary>
        /// <param name="userId">User id</param>
        /// <returns></returns>
        public string GetConfirmAnswer(string userId)
        {
            string answer = _dataContext.Connection.Query<string>("GetUserConfirmAnswer",
                                                                  new { userId = userId },
                                                                  commandType: CommandType.StoredProcedure).FirstOrDefault();
            return answer;
        }

        /// <summary>
        /// Check is answer right
        /// </summary>
        /// <param name="reminderUserAnswer">Reminder answer</param>
        /// <param name="userId">Use id</param>
        /// <returns></returns>
        public bool IsRightAnswerOnQuestion(string reminderUserAnswer, string userId)
        {

            bool isRightAnswer = _dataContext.Connection.Query<bool>("CheckReminderAnswer",
                                                                     new { userId = userId, userAnswer = reminderUserAnswer },
                                                                     commandType: CommandType.StoredProcedure).FirstOrDefault();
            return isRightAnswer;
        }


        /// <summary>
        /// Retrieves user details
        /// </summary>
        /// <param name="userId">User identifier</param>
        /// <returns>User details DTO</returns>
        public User_DetailsModel GetUserDetails(string userId)
        {
            return _dataContext.Connection
                .Query<User_DetailsModel>("RetrieveUserData", new { UserID = userId }, commandType: CommandType.StoredProcedure)
                .FirstOrDefault();
        }

        /// <summary>
        /// Get access levels
        /// </summary>
        /// <returns></returns>
        public IList<SelectListItem> GetUserLevelsOfAccess()
        {
            //Create SelectList of all levels for view 
            IList<SelectListItem> items = new List<SelectListItem>();


            int count = Properties.Settings.Default.StartValueForAccessLevel;
            for (int i = 0; i < Properties.Settings.Default.LevelsOfAccess.Count; i++)
            {
                var emptyItem = new SelectListItem
                {
                    Value = count.ToString(),
                    Text = Properties.Settings.Default.LevelsOfAccess[i]
                };
                items.Add(emptyItem);
                count++;
            }
            return items;

        }



        #endregion

        #region SAVE DATA
        /// <summary>
        /// Save new password
        /// </summary>
        /// <param name="userId">User id</param>
        /// <param name="newPassword">New password</param>
        /// <returns></returns>
        public bool SaveNewPassword(string userId, string newPassword)
        {
            bool isSamePassword = _dataContext.Connection.Query<bool>("SaveNewPassword",
                                            new { userId = userId, pass = newPassword },
                                            commandType: CommandType.StoredProcedure).FirstOrDefault();
            return isSamePassword;
        }


        public void UpdateUserStore(string userId, string @Clientid)
        {
             _dataContext.Connection.Execute("UpdateUserStore",
                                            new { userId = userId, @Clientid = Clientid },
                                            commandType: CommandType.StoredProcedure);
            
        }
        /// <summary>
        /// Save user datails
        /// </summary>
        /// <param name="model">user info model</param>
        public void SaveUserDetails(User_ConfidentialInfoModel model)
        {
            _dataContext.Connection.Execute("AddDetails",
                                        new
                                        {
                                            pass = model.Password,
                                            question = model.ReminderQuestion,
                                            answer = model.ReminderAnswer,
                                            userId = model.UserId
                                        }, commandType: CommandType.StoredProcedure);

        }

        #endregion

        
    }
}