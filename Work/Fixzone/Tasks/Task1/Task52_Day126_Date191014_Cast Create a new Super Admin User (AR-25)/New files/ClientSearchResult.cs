using System;

namespace CAST.Client
{
    /// <summary>
    /// Model for search results
    /// </summary>
    public class ClientsSearchResult
    {
        public int ClientID { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public string Postcode { get; set; }
        public string Contact { get; set; }
        public string Type { get; set; }
    }
}