using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CAST.Validation;

namespace CAST.Models.Administration
{
    public class AdministrationUserModel //: IValidatableObject
    {
        //public string ID { get; set; }

        /// <summary>
        /// User id for changing. New user id.
        /// </summary>
        [Required(ErrorMessage = "Input Employee ID")]
        [Display(Name = "Employee ID")]
        public string UserId { get; set; }
        
        /// <summary>
        /// Original user id. User id in database at present time.
        /// </summary>
        public string OriginalUserId { get; set; }


        [Required(ErrorMessage = "Input Name")]
        public string Name { get; set; }

        public string JobRole { get; set; }

        public bool IsEnabled { get; set; }
        
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        [PasswordValidation]
        public string Password { get; set; }


        [Display(Name = "Level")]
        public int UserLevel { get; set; }
        
        public int StoreNumber { get; set; }

        public DateTime? DateOfBirth { get; set; }

        //[Required]
        //[Range(1, 31, ErrorMessage = "Wrong Day")]
        public int Day { get; set; }

        //[Required]
        //[Range(1, 12, ErrorMessage = "Wrong Month")]
        public int Month { get; set; }

        //[Required]
        //[RegularExpression(@"[0-9].{3}", ErrorMessage = "Wrong Year")]
        public int Year { get; set; }

        /// <summary>
        /// using in administration part when changin user id
        /// </summary>
        public string PreviousUserId { get; set; }

        /// <summary>
        /// Access levels
        /// </summary>
        public IList<SelectListItem> LevelsOfAccess { get; set; }

        //public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        //{
        //    var errors = new List<ValidationResult>();
        //    try
        //    {
                
        //        var date = String.Format("{0:00}/{1:00}/{2:0000}", Day, Month, Year);
        //        var dateBirth = DateTime.ParseExact(date, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
        //        if(dateBirth>DateTime.Now) errors.Add(new ValidationResult("Can't be future date",new []{"Year"}));
               
        //    }
        //    catch
        //    {
        //        errors.Add(new ValidationResult("Check Date of Birth",new []{"Year"}));
        //    }

        //    return errors;
        //}

      
    }
}