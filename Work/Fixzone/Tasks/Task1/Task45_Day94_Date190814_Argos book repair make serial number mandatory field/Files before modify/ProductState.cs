using System;

namespace CAST.Sessions
{
    /// <summary>
    /// Class for working with Product info in Session
    /// </summary>
    [Serializable]
    public class ProductState
    {
        /// <summary>
        /// Model ID
        /// </summary>
        public int? ModelID {get; set; }

        /// <summary>
        /// Timestamp when the user clicked ‘Book Repair’ ID
        /// </summary>
        public DateTime? TimeBookRepairClick { get; set; }

        /// <summary>
        /// Current search criteria
        /// </summary>
        public string SearchCriteria { get; set; }

        /// <summary>
        /// Nodel number
        /// </summary>
        public string ModelNumber { get; set; }

        /// <summary>
        /// Model number
        /// </summary>
        public string ModelDescription { get; set; }

        /// <summary>
        /// Soft id
        /// </summary>
        public int? SoftId { get; set; }

        /// <summary>
        /// Item Condition
        /// </summary>
        public string OriginalCondition { get; set; }

        /// <summary>
        /// Serial number
        /// </summary>
        public string SerialNumber { get; set; }

        /// <summary>
        /// Transaction number
        /// </summary>
        public string TransactionNumber { get; set; }

        /// <summary>
        /// DOP
        /// </summary>
        public DateTime DateOfPurchase { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string RepairFaq { get; set; }

        /// <summary>
        /// Page number of search result
        /// </summary>
        public int PageNumOfSearchResult { get; set; }

        public string ItemCode { get; set; }

        public string Brand { get; set; }
        public string ModelBrand { get; set; }
    }

}