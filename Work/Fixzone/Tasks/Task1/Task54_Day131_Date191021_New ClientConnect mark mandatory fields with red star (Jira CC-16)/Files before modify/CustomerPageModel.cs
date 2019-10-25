using System.Collections.Generic;
using System.Web.Mvc;
using ClientConnect.ViewModels.Address;
using System.ComponentModel.DataAnnotations;

namespace ClientConnect.ViewModels.BookNewService
{
    public class CustomerPageModel
    {
        public CustomerPageModel()
        {
            ContactMethodList = new List<SelectListItem>();
            AdressesLists = new AddressModel();
            TitleList = new List<SelectListItem>();
            CountryList = new List<SelectListItem>();
            RetailClientList = new List<SelectListItem>();
        }

        /// <summary>
        /// Customer id
        /// </summary>
        public int CustomerId { get; set; }
       // public int OwnerCustomerId { get; set; }

        /// <summary>
        /// List of appeal
        /// </summary>
        public List<SelectListItem> TitleList { get; set; }
        
        /// <summary>
        /// List of Prefer
        /// </summary>
        public List<SelectListItem> ContactMethodList { get; set; }

        public string Title { get; set; }

        public string Forename { get; set; }

        public string Surname { get; set; }

        public string Postcode { get; set; }
        
        public string HouseNumber { get; set; }

        public string Addr1 { get; set; }

        public string Addr2 { get; set; }

        public string Addr3 { get; set; }

        public string Organization { get; set; }

        public string Department { get; set; }
        
        public string MobileTel { get; set; }

        public string LandlineTel { get; set; }
        
        public string Email { get; set; }
        
        
        
        
         [Display(Name = "Customer agrees to receive a survey from Sony after completion of this repair?")]
         public bool Customer_Survey { get; set; }

        public int ContactMethod { get; set; }

        public string Town { get; set; }
        
        public string County { get; set; }

        public string Country { get; set; }

        public List<SelectListItem> CountryList { get; set; }

        public AddressModel AdressesLists { get; set; }

        public List<SelectListItem> RetailClientList { get; set; }
        public int RetailClient { get; set; }
        public string RetailClientName { get; set; }
        public string ClientId { get; set; }

        public string CLIENTCUSTREF { get; set; }
    }
}