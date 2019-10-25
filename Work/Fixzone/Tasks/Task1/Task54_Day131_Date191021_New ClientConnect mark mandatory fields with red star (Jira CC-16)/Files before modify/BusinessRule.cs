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
        BookRepairIsVisible
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