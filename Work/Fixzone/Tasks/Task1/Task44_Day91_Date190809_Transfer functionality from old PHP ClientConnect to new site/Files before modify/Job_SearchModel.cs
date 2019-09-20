using System.Collections.Generic;
using System.ComponentModel;

namespace ClientConnect.Jobs
{
    /// <summary>
    /// Model for job search page
    /// </summary>
    public class Job_SearchModel
    {
        public Job_SearchModel()
        {
            SearchCriteria = string.Empty;
            CurrentPage = 1;
            FirstItemIndex = 0;
            TotalRecords = 0;
            LastItemIndex = 0;
            SearchResults = new List<Job_SearchResult>();
            AdvSearchCriteria = new AdvSearchCriteria();
        }

        /// <summary>
        /// Current search criteria
        /// </summary>
        public string SearchCriteria { get; set; }

        /// <summary>
        /// The index of the first displayed record in overall result set
        /// </summary>
        public int FirstItemIndex { get; set; }

        /// <summary>
        /// Current page
        /// </summary>
        public int CurrentPage { get; set; }

        /// <summary>
        /// The index of the last displayed record in overall result set 
        /// </summary>
        public int LastItemIndex { get; set; }

        /// <summary>
        /// Total number of records found
        /// </summary>
        public int TotalRecords { get; set; }

        /// <summary>
        /// Collection of job search results
        /// </summary>
        public List<Job_SearchResult> SearchResults { get; set; }

        /// <summary>
        /// Whether result set has previous data page
        /// </summary>
        public bool HasPreviousPage
        {
            get { return CurrentPage > 1; }
        }

        /// <summary>
        /// Whether result set has next data page
        /// </summary>
        public bool HasNextPage
        {
            get { return LastItemIndex  < TotalRecords; }
        }

        public AdvSearchCriteria AdvSearchCriteria { get; set; }
    }

  public  class AdvSearchCriteria
  {// customer name, customer reference, postcode, address, telephone number, or policy number

      public AdvSearchCriteria()
      {
          Surname = string.Empty;
          Postcode = string.Empty;
          ClientCustRef = string.Empty;
          PolicyNumber = string.Empty;
          Address = string.Empty;
          TelNo = string.Empty;
          JobId = string.Empty;
      }
      [DisplayName("Job Id")]
      public string JobId { get; set; }
        [DisplayName("Surname")]
        public string Surname { get; set; }
        [DisplayName("Postcode")]
        public string Postcode { get; set; }
        [DisplayName("Customer Reference")]
        public string ClientCustRef { get; set; }
       [DisplayName("Policy Number")]
        public string PolicyNumber { get; set; }
          [DisplayName("Address")]
        public string Address { get; set; }
       [DisplayName("Telephone")]
        public string TelNo { get; set; }
    }
}