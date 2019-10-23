using System.Linq;
using ClientConnect.Customer;
using ClientConnect.Infrastructure;
using ClientConnect.ViewModels.BookNewService;
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
            CascadeMode = CascadeMode.StopOnFirstFailure;

            //Title
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Select title");

            //Forename
            RuleFor(x => x.Forename)
                .NotEmpty().WithMessage("Input forename")
                .Length(0, 20).WithMessage("Maximum 20 characters");

            //Surname
            RuleFor(x => x.Surname)
                .NotEmpty().WithMessage("Input surname")
                .Length(0, 50).WithMessage("Maximum 50 characters");

            //Postcode 
            RuleFor(x => x.Postcode)
                .NotNull().WithMessage("Input postcode").When(x => x.Country != DefaultValues.IrelandCountryCode)
                .NotEmpty().WithMessage("Input postcode").When(x => x.Country != DefaultValues.IrelandCountryCode)
                .Must(postcode =>
                    {
                        if  (postcode.ToString().Trim().Replace(" ", string.Empty).Length > 8)
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

            //Addr1 
            RuleFor(x => x.Addr1)
                .NotNull().WithMessage("Input address")
                .NotEmpty().WithMessage("Input address")
                .Length(0, 60).WithMessage("Maximum 60 characters");

            //Addr2 
            RuleFor(x => x.Addr2)
                .Length(0, 60).WithMessage("Maximum 60 characters");

            //Addr3 
            RuleFor(x => x.Addr3)
                .Length(0, 60).WithMessage("Maximum 60 characters");

            //Town 
            RuleFor(x => x.Town)
                .NotNull().WithMessage("Input town")
                .NotEmpty().WithMessage("Input town")
                .Length(0, 60).WithMessage("Maximum 60 characters");

            ////County 
            //RuleFor(x => x.County)
            //    .NotNull().WithMessage("Input County")
            //    .NotEmpty().WithMessage("Input County")
            //    .Length(0, 40).WithMessage("Maximum 40 characters");

            //LandlineTel 
            RuleFor(x => x.LandlineTel)
                .NotEmpty().When(x => x.ContactMethod == (short)ContactTelephone).WithMessage("Please supply telephone")
                .NotEmpty().When(x => string.IsNullOrEmpty(x.MobileTel)).WithMessage("Please supply at least one phone number")
                .Length(0, 20).WithMessage("Maximum 20 characters");

            //Mobile 
            RuleFor(x => x.MobileTel)
                .NotEmpty().When(x => x.ContactMethod == (short)ContactSms).WithMessage("Please supply telephone number")
                .NotEmpty().When(x => string.IsNullOrEmpty(x.LandlineTel)).WithMessage("Please supply at least one phone number")
                .Length(0, 20).WithMessage("Maximum 20 characters")
                .Matches(new ValidationFormats().MobilePhone).WithMessage("Wrong format").When(x => x.Country != DefaultValues.IrelandCountryCode);

            //Email 
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Please supply email address")
                  .EmailAddress().WithMessage("A valid email is required")
                //.NotEmpty().When(x => x.ContactMethod == (short) ContactEmail).WithMessage("Input email")
                .Length(0, 64).WithMessage("Maximum 64 characters");
        }
    }
}