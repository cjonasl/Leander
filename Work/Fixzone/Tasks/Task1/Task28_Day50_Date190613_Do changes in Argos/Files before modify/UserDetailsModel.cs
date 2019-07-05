using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CAST.Validation;

namespace CAST.Models.User
{
    public class UserDetailsModel
    {
        /// <summary>
        /// User id
        /// </summary>
        [Required(ErrorMessage = "Input Employee ID")]
        [Display(Name = "Employee ID")]
        public string UserId { get; set; }

        /// <summary>
        /// User name
        /// </summary>
        [Required(ErrorMessage = "Input Name")]
        public string Name { get; set; }

        /// <summary>
        /// Role
        /// </summary>
        public string JobRole { get; set; }

        /// <summary>
        /// Is enabled
        /// </summary>
        public bool IsEnabled { get; set; }
        
        /// <summary>
        /// Password
        /// </summary>
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        /// <summary>
        /// User level access
        /// </summary>
        [Display(Name = "Level")]
        public int UserLevel { get; set; }

        /// <summary>
        /// Memorable question
        /// </summary>
        [Display(Name = "Memorable Question")]
        [StringLength(60, ErrorMessage = "The question should't be less of 4 and more than 60", MinimumLength = 3)]
        public string ReminderQuestion { get; set; }

        /// <summary>
        /// Answer
        /// </summary>
        [Display(Name = "Answer")]
        [StringLength(20, ErrorMessage = "You answer should'n be less of 4  and more then 20", MinimumLength = 3)]
        public string ReminderAnswer { get; set; }

        /// <summary>
        /// Confirm password
        /// </summary>
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        [Display(Name = "Confirm Password")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

        /// <summary>
        /// Store number
        /// </summary>
        public int StoreNumber { get; set; }


        public DateTime? DateOfBirth { get; set; }
        
        [Range(1, 31, ErrorMessage = "Wrong Day")]
        public int Day { get; set; }

        [Range(1, 12, ErrorMessage = "Wrong Month")]
        public int Month { get; set; }
        
        [Range(2000, 2200, ErrorMessage = "Wrong Year")]
        public int Year { get; set; }


    }

    
}