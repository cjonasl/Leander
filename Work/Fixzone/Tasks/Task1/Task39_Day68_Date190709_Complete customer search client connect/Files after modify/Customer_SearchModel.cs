using System.Collections.Generic;
using System.ComponentModel;

namespace ClientConnect.Customer
{
    /// <summary>
    /// Model for job search page
    /// </summary>
    public class Customer_SearchModel
    {
        public Customer_SearchModel()
        {
            SearchCriteria = string.Empty;
            CurrentPage = 1;
            FirstItemIndex = 0;
            TotalRecords = 0;
            LastItemIndex = 0;
            SearchResults = new List<Customer_SearchResult>();
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
        public List<Customer_SearchResult> SearchResults { get; set; }

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
          UseAndInWhereCondition = null;
      }

      public AdvSearchCriteria(string surname, string postcode, string clientCustRef, string policyNumber, string address, string telNo, bool useAndInWhereCondition)
      {
          Surname = surname;
          Postcode = postcode;
          ClientCustRef = clientCustRef;
          PolicyNumber = policyNumber;
          Address = address;
          TelNo = telNo;
          UseAndInWhereCondition = useAndInWhereCondition;
      }
     
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
        public bool? UseAndInWhereCondition { get; set; } //Whether to use "and" or "or" between the filter conditions, i.e. all conditions must be true or at least one condition true
    }

}