using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AddressBook
{
    public class ChangePassword
    {
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }

        public ChangePassword() { }
        public ChangePassword(string oldPassword, string newPassword)
        {
            this.OldPassword = oldPassword;
            this.NewPassword = newPassword;
        }
    }
}