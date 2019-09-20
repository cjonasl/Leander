using System;

namespace ClientConnect.Models.Job
{
    [Serializable]
    public class Job_SessionModel
    {
        /// <summary>
        /// Jobs search criteria
        /// </summary>
        public string SearchCriteria { get; set; }

        /// <summary>
        /// Job identifier
        /// </summary>
        public int ServiceId { get; set; }

        /// <summary>
        /// Page number
        /// </summary>
        public int PageNumber { get; set; }

        public string Surname { get; set; }
       
        public string Postcode { get; set; }
       
        public string ClientCustRef { get; set; }
    
        public string PolicyNumber { get; set; }
      
        public string Address { get; set; }
     
        public string TelNo { get; set; }
        public string SearchJobId { get; set; }
       
    }
}