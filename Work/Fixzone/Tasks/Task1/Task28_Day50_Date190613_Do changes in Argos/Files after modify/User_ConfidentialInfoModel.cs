using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using CAST.Validation;
using System;

namespace CAST.ViewModels.User
{
    public class User_ConfidentialInfoModel
    {
        /// <summary>
        /// User id
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Password
        /// </summary>
        [Required(ErrorMessage = "Input Password")]
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        [PasswordValidation]
        public string Password { get; set; }

        /// <summary>
        /// Memorable question
        /// </summary>
        [Required(ErrorMessage = "Input question")]
        [Display(Name = "Memorable Question")]
        [StringLength(60, ErrorMessage = "The question should't be less of 4 and more than 60", MinimumLength = 3)]
        public string ReminderQuestion { get; set; }

        /// <summary>
        /// Answer
        /// </summary>
        [Required(ErrorMessage = "Input answer")]
        [Display(Name = "Answer")]
        [StringLength(20, ErrorMessage = "Your answer must be between 4 and 20 characters.", MinimumLength = 3)]
        public string ReminderAnswer { get; set; }

        /// <summary>
        /// Confirm password
        /// </summary>
        [Required(ErrorMessage = "Confirm pasword")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        [Display(Name = "Confirm Password")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
    }

    public class User_AccountInfoModel
    {
        /// <summary>
        /// User id
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Memorable question
        /// </summary>
        [Required(ErrorMessage = "Memorable Question is required")]
        [Display(Name = "Memorable Question")]
        [StringLength(60, ErrorMessage = "The question should't be less of 4 and more than 60", MinimumLength = 3)]
        public string ReminderQuestion { get; set; }

        /// <summary>
        /// Answer
        /// </summary>
        [Required(ErrorMessage = "Answer is required")]
        [Display(Name = "Answer")]
        [StringLength(20, ErrorMessage = "Your answer must be between 4 and 20 characters.", MinimumLength = 3)]
        public string ReminderAnswer { get; set; }

        [Required(ErrorMessage = "Day is required")]
        [Range(1, 31, ErrorMessage = "Incorrect given day")]
        public int Day { get; set; }

        [Required(ErrorMessage = "Month is required")]
        [Range(1, 12, ErrorMessage = "Incorrect given month")]
        public int Month { get; set; }

        [Required(ErrorMessage = "Year is required")]
        [Range(1900, 2100, ErrorMessage = "Incorrect given year")]
        public int Year { get; set; }

        public DateTime? DateOfBirth { get; set; }
    }
}