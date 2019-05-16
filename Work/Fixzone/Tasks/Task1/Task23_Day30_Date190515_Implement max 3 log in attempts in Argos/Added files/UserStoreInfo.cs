using System;

namespace CAST.ViewModels.User
{
    [Serializable]
    public class UserStoreInfo
    {
        /// <summary>
        /// User can log in
        /// </summary>
        public bool UserCanLogIn { get; set; }
        
        /// <summary>
        /// Store Id number
        /// </summary>
        public int? UserStoreID { get; set; }

        /// <summary>
        /// Store address
        /// </summary>
        public string UserStoreName { get; set; }
    }
}