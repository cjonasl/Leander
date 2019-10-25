using ClientConnect.FzOnlineBooking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClientConnect.Services
{
    [Serializable]
    public class AvailableRequestDetails
    {
        public string PreferredVisitDate { get; set; }

        public string Postcode { get; set; }

        public bool BookImmediately { get; set; }

        public int RequestedEngineerID { get; set; }

        public bool BookToManual { get; set; }

        public int BookingOptions { get; set; }

        public int EngineerID { get; set; }

        public string ApplianceCode { get; set; }

        public string Skill { get; set; }
    }
    [Serializable]
    public class AvailableResponseDetails
    {

        public AvailableResponseDetails()
        {
            AvailableList = new List<TOneAvailableResult>();
        }

        /// <remarks/>
        public int ErrorCode { get; set; }

        /// <remarks/>
        public string ErrorText { get; set; }

        /// <remarks/>
        public bool RequestSuccess { get; set; }


        public List<TOneAvailableResult> AvailableList
        { get; set; }

    }
    [Serializable]
    public class TOneAvailableResult
    {
        /// <remarks/>
        public string EngineerName { get; set; }

        /// <remarks/>
        public int EngineerID { get; set; }

        /// <remarks/>
        public int Calls { get; set; }

        /// <remarks/>
        public double Distance { get; set; }

        /// <remarks/>
        public int Priority { get; set; }

        public DateTime EventDate { get; set; }

        /// <remarks/>
        public string Description { get; set; }
    }

    [Serializable]
    public class OneBookOptionResult
    {
        public string EngineerName
        {
            get;
            set;
        }

        /// <remarks/>
        public int EngineerID
        {
            get;
            set;
        }

        /// <remarks/>
        public int Calls
        {
            get;
            set;
        }

        /// <remarks/>
        public double Distance
        {
            get;
            set;
        }

        /// <remarks/>
        public int Priority
        {
            get;
            set;
        }

        /// <remarks/>
        public System.DateTime EventDate
        {
            get;
            set;
        }

        /// <remarks/>
        public string Description
        {
            get;
            set;
        }
    }
    [Serializable]
    public class ResponseDetails
    {
        public int ErrorCode { get; set; }

        /// <remarks/>
        public string ErrorText { get; set; }

        /// <remarks/>
        public bool RequestSuccess { get; set; }

        /// <remarks/>
        public int AllocatedDiaryEntID { get; set; }

        /// <remarks/>
        public int AllocatedServiceID
        {
            get;
            set;
        }

        /// <remarks/>
        public int AllocatedEngineerID { get; set; }

        /// <remarks/>
        public List<OneBookOptionResult> BookingOptionResult { get; set; }
        public ResponseDetails()
        {
            BookingOptionResult = new List<OneBookOptionResult>();
        }

    }
    [Serializable]
    public class RequestDetails
    {
        public string SaediID { get; set; }

        /// <remarks/>
        public int CurrentServiceID { get; set; }

        /// <remarks/>
        public int ClientID { get; set; }

        /// <remarks/>
        public string ClientSAEDIID { get; set; }

        /// <remarks/>
        public string ClientPassword { get; set; }

        /// <remarks/>
        public string RequestedDate { get; set; }

        /// <remarks/>
        public string RequestedTime { get; set; }

        /// <remarks/>
        public int RequestedDiaryMinutes { get; set; }

        /// <remarks/>
        public int RequestedEngineerID { get; set; }

        /// <remarks/>
        public string EngineerSAEDIID { get; set; }

        /// <remarks/>
        public bool BookImmediately { get; set; }

        /// <remarks/>
        public bool CreateServiceCall { get; set; }

        /// <remarks/>
        public int BookingOptions { get; set; }

        /// <remarks/>
        public string Postcode { get; set; }

        /// <remarks/>
        public string AddressLine1 { get; set; }

        /// <remarks/>
        public string Town { get; set; }

        /// <remarks/>
        public string Skill { get; set; }

        /// <remarks/>
        public string ApplianceCode { get; set; }

      
        private bool _uniqueDates = true;

        public bool UniqueDates
        {
            get { return _uniqueDates; }
            set { _uniqueDates = value; }
        }
    }
}
