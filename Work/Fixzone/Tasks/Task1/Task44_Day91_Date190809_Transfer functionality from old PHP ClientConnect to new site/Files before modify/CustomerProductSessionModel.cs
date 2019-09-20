﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ClientConnect.ViewModels.Address;

namespace ClientConnect.Models.CustomerProduct
{
    
    [Serializable]
    public class CustomerProductSessionModel
    {
        public int CustProductId { get; set; }
        public string ApplianceCD { get; set; }
        public int ModelId { get; set; }
        public string Skills { get; set; }
        public int CustomerId { get; set; }
        public string AuthNo { get; set; }
        public DateTime? DateofPurchase { get; set; }
        public string SerialNumber { get; set; }
        public DateTime? MechanicalCoverStarts { get; set; }
        //public string Title { get; set; }

        //public string Forename { get; set; }

        //public string Surname { get; set; }

        //public string Postcode { get; set; }

        //public string HouseNumber { get; set; }

        //public string Addr1 { get; set; }

        //public string Addr2 { get; set; }

        //public string Addr3 { get; set; }

        //public string Organization { get; set; }

        //public string Department { get; set; }

        //public string MobileTel { get; set; }

        //public string LandlineTel { get; set; }

        //public string Email { get; set; }

        //public int ContactMethod { get; set; }

        //public string Town { get; set; }

        //public string County { get; set; }

        //public string Country { get; set; }

        //public string ClosestStoreName { get; set; }

        //public string ClosestStoreAddress { get; set; }

        //public string ClosestStoreDistance { get; set; }

        //public int ClosestStoreId { get; set; }

        //public string CollectPostcode { get; set; }

        //public string CollectHouseNumber { get; set; }

        //public string CollectAddr1 { get; set; }

        //public string CollectAddr2 { get; set; }

        //public string CollectAddr3 { get; set; }

        //public string CollectOrganization { get; set; }

        //public string CollectDepartment { get; set; }

        //public string CollectTown { get; set; }

        //public string CollectCounty { get; set; }

        //public string CLIENTCUSTREF { get; set; }
        //public string SearchCriteria { get; set; }

   

        ///// <summary>
        ///// Page number
        ///// </summary>
        //public int PageNumber { get; set; }

        
    }
   
}

