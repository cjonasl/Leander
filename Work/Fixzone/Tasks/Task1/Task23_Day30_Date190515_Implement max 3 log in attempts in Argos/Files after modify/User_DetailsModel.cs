using System;

namespace CAST.ViewModels.User
{
    [Serializable]
    public class User_DetailsModel
    {

        /// <summary>
        /// User Id 
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// User password
        /// </summary>
        public string UserPassword { get; set; }


        /// <summary>
        /// User full name 
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Store Id number
        /// </summary>
        public int? UserStoreID { get; set; }

        /// <summary>
        /// Store address
        /// </summary>
        public string UserStoreName { get; set; }

        /// <summary>
        /// Flag of priority booking
        /// </summary>
        public bool ClientPriorityBooking { get; set; }

        /// <summary>
        /// Password expired
        /// </summary>
        public bool PasswordExpired { get; set; }

        /// <summary>
        /// User level access
        /// </summary>
        public int UserLevel { get; set; }

        /// <summary>
        /// Is user enabled
        /// </summary>
        public int Enabled { get; set; }

        /// <summary>
        /// Is password empty or not
        /// </summary>
        public int IsPasswordEmpty { get; set; }

        /// <summary>
        /// User computer name
        /// </summary>
        public string UserComputerName { get; set; }

        /// <summary>
        /// User available memory
        /// </summary>
        public string UserMemoryAvailable { get; set; }

        /// <summary>
        /// Is need to do diagnostic or not
        /// </summary>
        public int RunAutoDiagnostic { get; set; }

        /// <summary>
        /// Is need to get system info of client
        /// </summary>
        public int IsGetSystemInfo
        {
            get
            {
                if (string.IsNullOrEmpty(UserComputerName)) return 1;
                return 0;
            }
        }
        public int GroupID
        {
            get;
            set;
        }

        /// <summary>
        /// Last active date
        /// </summary>
        public DateTime? Lastacdt { get; set; }
        public string ReminderQuestion { get; set; }
        public string ReminderAnswer { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public int? NumberOfLogInFailures { get; set; }
    }
}