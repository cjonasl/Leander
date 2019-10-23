using System.Linq;
using ClientConnect.Customer;
using ClientConnect.Infrastructure;
using ClientConnect.ViewModels.BookNewService;
using ClientConnect.Services;
using ClientConnect.Configuration;
using ClientConnect.Home;
using FluentValidation;

namespace ClientConnect.Validation
{
    public class BookNewService_CustomerPageValdation : AbstractValidator<CustomerPageModel>
    {
        private RuleSetKeys _ruleSetKeys = new RuleSetKeys();

        private int ContactSms
        {
            get { return ContactMethod.ContactMethod.SMS; }
        }

        private int ContactEmail
        {
            get { return ContactMethod.ContactMethod.Email; }
        }

        private int ContactTelephone
        {
            get { return ContactMethod.ContactMethod.Telephone; }
        }

        public BookNewService_CustomerPageValdation()
        {
            HomeService homeService = (HomeService)Ioc.Get<HomeService>();
            StoreService storeService = (StoreService)Ioc.Get<StoreService>();
            var businessRules = homeService.GetBusinessRuleList(storeService.StoreId);

            bool retailClientIsMandatory = BusinessRule.GetValue(businessRules, BusinessRuleKey.CustomerPage_RetailClient_Is_Mandatory);
            bool cLIENTCUSTREFIsMandatory = BusinessRule.GetValue(businessRules, BusinessRuleKey.CustomerPage_CLIENTCUSTREF_Is_Mandatory);
            bool titleIsMandatory = BusinessRule.GetValue(businessRules, BusinessRuleKey.CustomerPage_Title_Is_Mandatory);
            bool forenameIsMandatory = BusinessRule.GetValue(businessRules, BusinessRuleKey.CustomerPage_Forename_Is_Mandatory);
            bool surnameIsMandatory = BusinessRule.GetValue(businessRules, BusinessRuleKey.CustomerPage_Surname_Is_Mandatory);
            bool postcodeIsMandatory = BusinessRule.GetValue(businessRules, BusinessRuleKey.CustomerPage_Postcode_Is_Mandatory);
            bool addr1IsMandatory = BusinessRule.GetValue(businessRules, BusinessRuleKey.CustomerPage_Addr1_Is_Mandatory);
            bool addr2IsMandatory = BusinessRule.GetValue(businessRules, BusinessRuleKey.CustomerPage_Addr2_Is_Mandatory);
            bool addr3IsMandatory = BusinessRule.GetValue(businessRules, BusinessRuleKey.CustomerPage_Addr3_Is_Mandatory);
            bool townIsMandatory = BusinessRule.GetValue(businessRules, BusinessRuleKey.CustomerPage_Town_Is_Mandatory);
            bool countyIsMandatory = BusinessRule.GetValue(businessRules, BusinessRuleKey.CustomerPage_County_Is_Mandatory);
            bool countryIsMandatory = BusinessRule.GetValue(businessRules, BusinessRuleKey.CustomerPage_Country_Is_Mandatory);
            bool Tel1IsMandatory = BusinessRule.GetValue(businessRules, BusinessRuleKey.CustomerPage_Tel1_Is_Mandatory);
            bool Tel2IsMandatory = BusinessRule.GetValue(businessRules, BusinessRuleKey.CustomerPage_Tel2_Is_Mandatory);
            bool emailIsMandatory = BusinessRule.GetValue(businessRules, BusinessRuleKey.CustomerPage_Email_Is_Mandatory);
            bool contactMethodIsMandatory = BusinessRule.GetValue(businessRules, BusinessRuleKey.CustomerPage_ContactMethod_Is_Mandatory);
            
            CascadeMode = CascadeMode.StopOnFirstFailure;

            //RetailClient
            if (retailClientIsMandatory)
            {
                RuleFor(x => x.RetailClient).NotEmpty().WithMessage("Select retail client");
            }

            //Account number (refers to CLIENTCUSTREF)
            if (cLIENTCUSTREFIsMandatory)
            {
                RuleFor(x => x.CLIENTCUSTREF).NotEmpty().WithMessage("Input account number");
            }

            //Title
            if (titleIsMandatory)
            {
                RuleFor(x => x.Title).NotEmpty().WithMessage("Select title");
            }

            //Forename
            if (forenameIsMandatory)
            {
                RuleFor(x => x.Forename)
                    .NotEmpty().WithMessage("Input forename")
                    .Length(0, 20).WithMessage("Maximum 20 characters");
            }
            else
            {
                RuleFor(x => x.Forename).Length(0, 20).WithMessage("Maximum 20 characters");
            }

            //Surname
            if (surnameIsMandatory)
            {
                RuleFor(x => x.Surname)
                    .NotEmpty().WithMessage("Input surname")
                    .Length(0, 50).WithMessage("Maximum 50 characters");
            }
            else
            {
                RuleFor(x => x.Surname).Length(0, 50).WithMessage("Maximum 50 characters");
            }

            //Postcode 
            if (postcodeIsMandatory)
            {
                RuleFor(x => x.Postcode)
                    .NotNull().WithMessage("Input postcode").When(x => x.Country != DefaultValues.IrelandCountryCode)
                    .NotEmpty().WithMessage("Input postcode").When(x => x.Country != DefaultValues.IrelandCountryCode)
                    .Must(postcode =>
                        {
                            if (postcode.ToString().Trim().Replace(" ", string.Empty).Length > 8)
                                return false;
                            return true;
                        }).When(x => x.Country != DefaultValues.IrelandCountryCode).WithMessage("Maximum 8 characters")
                    .Must((model, value) =>
                        {

                            // validation on one space
                            value = value.ToString().TrimStart().TrimEnd();
                            if (value.ToString().ToCharArray().Where(x => x == ' ').Count() != 1)
                            {
                                return false;
                            }
                            return true;
                        }).WithMessage("Postcode Must Contain a Space");
            }
            else
            {
                RuleFor(x => x.Postcode)
                 .Must(postcode =>
                 {
                    if (postcode.ToString().Trim().Replace(" ", string.Empty).Length > 8)
                       return false;
                    return true;
                 }).When(x => x.Country != DefaultValues.IrelandCountryCode).WithMessage("Maximum 8 characters")
                .Must((model, value) =>
                {
                  //validation on one space
                  value = value.ToString().TrimStart().TrimEnd();
                 if (value.ToString().ToCharArray().Where(x => x == ' ').Count() != 1)
                 {
                     return false;
                 }
                   return true;
                 }).WithMessage("Postcode Must Contain a Space");
            }

            //Addr1 
            if (addr1IsMandatory)
            {
                RuleFor(x => x.Addr1)
                    .NotNull().WithMessage("Input address")
                    .NotEmpty().WithMessage("Input address")
                    .Length(0, 60).WithMessage("Maximum 60 characters");
            }
            else
            {
                RuleFor(x => x.Addr1)
                    .Length(0, 60).WithMessage("Maximum 60 characters");
            }

            //Addr2 
            if (addr2IsMandatory)
            {
                RuleFor(x => x.Addr2)
                    .NotNull().WithMessage("Input address")
                    .NotEmpty().WithMessage("Input address")
                    .Length(0, 60).WithMessage("Maximum 60 characters");
            }
            else
            {
                RuleFor(x => x.Addr1)
                    .Length(0, 60).WithMessage("Maximum 60 characters");
            }

            //Addr3 
            if (addr3IsMandatory)
            {
                RuleFor(x => x.Addr3)
                    .NotNull().WithMessage("Input address")
                    .NotEmpty().WithMessage("Input address")
                    .Length(0, 60).WithMessage("Maximum 60 characters");
            }
            else
            {
                RuleFor(x => x.Addr1)
                    .Length(0, 60).WithMessage("Maximum 60 characters");
            }

            //Town 
            if (townIsMandatory)
            {
                RuleFor(x => x.Town)
                    .NotNull().WithMessage("Input town")
                    .NotEmpty().WithMessage("Input town")
                    .Length(0, 60).WithMessage("Maximum 60 characters");
            }
            else
            {
                RuleFor(x => x.Town).Length(0, 60).WithMessage("Maximum 60 characters");
            }

            //County
            if (countyIsMandatory)
            {
                RuleFor(x => x.Town)
                    .NotNull().WithMessage("Input county")
                    .NotEmpty().WithMessage("Input county");
            }

            //Country
            if (countryIsMandatory)
            {
                RuleFor(x => x.Town)
                    .NotNull().WithMessage("Input contry")
                    .NotEmpty().WithMessage("Input country");
            }

            //Tel1
            if (Tel1IsMandatory)
            {
                RuleFor(x => x.Tel1)
                    .NotNull().WithMessage("Please supply telephone")
                    .NotEmpty().WithMessage("Please supply telephone")
                    .Length(0, 20).WithMessage("Maximum 20 characters");
            }
            else
            {
                RuleFor(x => x.Tel1).Length(0, 20).WithMessage("Maximum 20 characters");
            }

            //Tel2
            if (Tel2IsMandatory)
            {
                RuleFor(x => x.Tel2)
                    .NotNull().WithMessage("Please supply telephone")
                    .NotEmpty().WithMessage("Please supply telephone")
                    .Length(0, 20).WithMessage("Maximum 20 characters");
            }
            else
            {
                RuleFor(x => x.Tel2).Length(0, 20).WithMessage("Maximum 20 characters");
            }

            //Email 
            if (emailIsMandatory)
            {
                RuleFor(x => x.Email)
                    .NotEmpty().WithMessage("Please supply email address")
                    .EmailAddress().WithMessage("A valid email is required")
                    .Length(0, 64).WithMessage("Maximum 64 characters");
            }
            else
            {
                RuleFor(x => x.Email)
                   .EmailAddress().WithMessage("A valid email is required")
                   .Length(0, 64).WithMessage("Maximum 64 characters");
            }
        }
    }
}