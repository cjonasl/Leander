using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ClientConnect.ViewModels.Address;

namespace ClientConnect.Models.Customer
{
    
    [Serializable]
    public class CustomerSessionResultModel
    {
        public CustomerSessionResultModel()
        {
            Applianceaddress = new Applianceaddress();
        }
        public Applianceaddress Applianceaddress { get; set; }
        public int CustomerId { get; set; }
        public string Title { get; set; }

        public string Forename { get; set; }

        public string Surname { get; set; }

        public string Postcode { get; set; }

        public string PolicyNumber { get; set; }

        public string TelNo { get; set; }

        public string ClientCustRef { get; set; }

        public string Address { get; set; }

        public string HouseNumber { get; set; }

        public string Addr1 { get; set; }

        public string Addr2 { get; set; }

        public string Addr3 { get; set; }

        public string Organization { get; set; }

        public string Department { get; set; }

        public string Tel1 { get; set; }

        public string Tel2 { get; set; }

        public string Email { get; set; }

        public int ContactMethod { get; set; }

        public string Town { get; set; }

        public string County { get; set; }

        public string Country { get; set; }

        public string ClosestStoreName { get; set; }

        public string ClosestStoreAddress { get; set; }

        public string ClosestStoreDistance { get; set; }

        public int ClosestStoreId { get; set; }

        public string CollectPostcode { get; set; }

        public string CollectHouseNumber { get; set; }

        public string CollectAddr1 { get; set; }

        public string CollectAddr2 { get; set; }

        public string CollectAddr3 { get; set; }

        public string CollectOrganization { get; set; }

        public string CollectDepartment { get; set; }

        public string CollectTown { get; set; }

        public string CollectCounty { get; set; }

        public string CLIENTCUSTREF { get; set; }
        public string SearchCriteria { get; set; }

        public int RetailClient { get; set; }
        public string RetailClientName { get; set; }
        /// <summary>
        /// Page number
        /// </summary>
        public int PageNumber { get; set; }

        public bool FromIndex { get; set; }
        public bool UseAndInWhereCondition { get; set; }
    }
    [Serializable]
    public class Applianceaddress
    {
        public string Postcode { get; set; }

        public string Town { get; set; }

        public string Addr1 { get; set; }

        public string Addr2 { get; set; }

        public string Addr3 { get; set; }
        public string County { get; set; }

        public string Country { get; set; }
        public string Title { get; set; }

        public string Forename { get; set; }

        public string Surname { get; set; }
    }
}

