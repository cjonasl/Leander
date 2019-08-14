namespace CAST.Products
{
    /// <summary>
    /// Class for result of search
    /// </summary>
    public class Product_SearchResult
    {
        /// <summary>
        /// Item code of product
        /// </summary>
        public string ItemCode { get; set; }

        /// <summary>
        /// Description of product
        /// </summary>
        public string Descr { get; set; }

        /// <summary>
        /// Brand of product
        /// </summary>
        public string Brand { get; set; }

        /// <summary>
        /// Model of product
        /// </summary>
        public string Model { get; set; }

        /// <summary>
        /// ID of product model
        /// </summary>
        public string ModelID { get; set; }
        
        /// <summary>
        /// Start element
        /// </summary>
        public int StartElem { get; set; }
        
        /// <summary>
        /// Elements count
        /// </summary>
        public int ElemCount { get; set; }
        
        /// <summary>
        /// Last element
        /// </summary>
        public int LastElem { get; set; }

        /// <summary>
        /// Is product have alternative product list
        /// </summary>
        public bool AlternativeFlag { get; set; }

        /// <summary>
        /// Original model if item is alternative
        /// </summary>
        public string originalModel { get; set; }

    }
}