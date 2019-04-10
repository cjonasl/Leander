using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace CAST.Validation
{
    public class PasswordValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
            {
                // Length validation
                if ((value.ToString().Length < 6) || (value.ToString().Length > 10))
                {
                    return new ValidationResult("Password should be minimum 6 and not more than 10");
                }
                
                // Check password
                if (Regex.IsMatch((string) value, @"^.*(?=.*\d)(?=.*[a-zA-Z]).*$"))
                    return ValidationResult.Success;
                return new ValidationResult("Wrong password format.");
            }
            return ValidationResult.Success;
        }
    }
}