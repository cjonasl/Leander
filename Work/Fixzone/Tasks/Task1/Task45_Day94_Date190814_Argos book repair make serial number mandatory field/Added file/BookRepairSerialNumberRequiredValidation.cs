using System;
using System.Configuration;
using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Web;
using CAST.Sessions;

namespace CAST.Validation
{
    public class BookRepairSerialNumberRequiredValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var productInfo = HttpContext.Current.Session["ProductInfo"];

            if (productInfo != null)
            {
                ArrayList v = new ArrayList(ConfigurationManager.AppSettings["BookRepairSerialNumberRequiredForRepairIDs"].Split(','));

                if (v.IndexOf((((ProductState)productInfo).RepairID).ToString()) >= 0 && (value == null || value.ToString().Trim() == ""))
                    return new ValidationResult("Input serial number");
            }

            return ValidationResult.Success;
        }
    }
}