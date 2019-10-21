using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CAST.Administration;
using CAST.ViewModels.Share;
using System.ComponentModel;
using CAST.Client;

namespace CAST.Models.Client
{
    public class ClientTableModel
    {
        /// <summary>
        /// Total users count
        /// </summary>
        public int ElemCount { get; set; }

        /// <summary>
        /// Number of starting record in users list
        /// </summary>
        public int StartElem { get; set; }

        /// <summary>
        /// Number of last record in users list
        /// </summary>
        public int EndElem { get; set; }

        public List<ClientsSearchResult> SearchResults { set; get; }

        private PagingInfo _pagiatorInfo;
        public PagingInfo PaginatorInfo
        {
            get { return _pagiatorInfo ?? (_pagiatorInfo = new PagingInfo()); }
            set { _pagiatorInfo = value; }
        }
    }

    public class Client_SearchModel
    {
        public string SearchType { get; set; } //AND or OR
        public string SearchTypeAnd { get; set; } //Empty string or checked
        public string SearchTypeOr { get; set; } //Empty string or checked

        [DisplayName("ClientID")]
        public string ClientID { get; set; }

        [DisplayName("Name")]
        public string Name { get; set; }

        [DisplayName("Location")]
        public string Location { get; set; }

        [DisplayName("Postcode")]
        public string Postcode { get; set; }

        [DisplayName("Contact")]
        public string Contact { get; set; }

        [DisplayName("Type")]
        public string ClientType { get; set; } //Callcenter, Store or Both

        public string ClientTypeCallcenter { get; set; } //Empty string or checked
        public string ClientTypeStore { get; set; } //Empty string or checked
        public string ClientTypeBoth { get; set; } //Empty string or checked

        public ClientTableModel ClientTable { get; set; }

        public Client_SearchModel()
        {
            SearchType = string.Empty;
            SearchTypeAnd = "checked";
            SearchTypeOr = string.Empty;
            ClientID = string.Empty;
            Name = string.Empty;
            Location = string.Empty;
            Postcode = string.Empty;
            Contact = string.Empty;
            ClientType = string.Empty;
            ClientTypeCallcenter = string.Empty;
            ClientTypeStore = string.Empty;
            ClientTypeBoth = "checked";
            ClientTable = null;
        }
    }
}