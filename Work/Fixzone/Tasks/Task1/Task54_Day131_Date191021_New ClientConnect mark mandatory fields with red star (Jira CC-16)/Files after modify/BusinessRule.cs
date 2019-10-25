using ClientConnect.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ClientConnect.Home
{
    public class BusinessRule
    {
        public string Key { get; set; }
        public bool Checked { get; set; }
        public string Value { get; set; }

        public static bool GetValue(List<BusinessRule> list, BusinessRuleKey key)
        {
            bool v;
            
            try
            {
                v = list.Where(x => x.Key == key.ToString()).FirstOrDefault().Checked;
            }
            catch
            {
                v = false;
            }

            return v;
        }
    }

    public enum BusinessRuleKey
    {
        ShowOrderNumber,
        ShowPNC,
        LowCostCal, ReplacementCover,
        NOAppliancePage,
        ShowAppointmentreason,
        Offlinebooking,
        SubmitTemplate,
        MobileTheft,
        ManufactWarranty,
        AdditionalJob,
        NOApplianceWarrantyInfo,
        RestrictedAddressChange,
        BookJobAllWarranty,
        StopBookingClientModelMissing,
        PartOrder,
        WarrantyfromDOP,
        DOPMandatory,
        ShowAccountNumber,
        ShowRegno,
        BlockSupplierList,
        ProductSearchIsVisible,
        JobSearchIsVisible,
        CustomerSearchIsVisible,
        JobStatusesIsVisible,
        BookRepairIsVisible,
        CustomerPage_RetailClient_Is_Mandatory,
        CustomerPage_CLIENTCUSTREF_Is_Mandatory,
        CustomerPage_Title_Is_Mandatory,
        CustomerPage_Forename_Is_Mandatory,
        CustomerPage_Surname_Is_Mandatory,
        CustomerPage_Postcode_Is_Mandatory,
        CustomerPage_Addr1_Is_Mandatory,
        CustomerPage_Addr2_Is_Mandatory,
        CustomerPage_Addr3_Is_Mandatory,
        CustomerPage_Town_Is_Mandatory,
        CustomerPage_County_Is_Mandatory,
        CustomerPage_Country_Is_Mandatory,
        CustomerPage_Tel1_Is_Mandatory,
        CustomerPage_Tel2_Is_Mandatory,
        CustomerPage_Email_Is_Mandatory,
        CustomerPage_ContactMethod_Is_Mandatory
    }

    public class SpecialJob
    {
        public int JobTypeid { get; set; }
        public JobType JobType
        {
            get
            {
                if (JobTypeid > 0)// && string.IsNullOrEmpty(Collectionref) && (Collectionref.Trim().Length==0))
                    return ((JobType)JobTypeid);

                    //  return true;
                else
                    return JobType.Defaulttype;
            }
        }
        public string EngDesc { get; set; }
        public int EngId { get; set; }
        public string VisitType { get; set; }
        public int StatusId { get; set; }
        public string Skill { get; set; }
    }
}