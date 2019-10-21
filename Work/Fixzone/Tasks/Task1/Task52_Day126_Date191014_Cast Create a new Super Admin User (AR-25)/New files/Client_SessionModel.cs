using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CAST.Models.Client
{
    [Serializable]
    public class Client_SessionModel
    {
        public string SearchType { get; set; } //AND or OR
        public string ClientID { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public string Postcode { get; set; }
        public string Contact { get; set; }
        public string ClientType { get; set; } //Callcenter, Store or Both
    }
}