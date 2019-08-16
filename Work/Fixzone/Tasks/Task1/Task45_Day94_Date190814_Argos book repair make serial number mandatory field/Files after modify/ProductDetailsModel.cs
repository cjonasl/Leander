using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CAST.Products;
using CAST.Properties;

namespace CAST.Models.Product
{
    public class ProductDetailsModel
    {
        // <summary>
        /// Constructor
        /// </summary>
        public ProductDetailsModel()
        {
            ProductServices = new ProductServicesModel();
            AdditioalLinks = new List<Product_AdditServiceModel>();
        }

        /// <summary>
        /// Item code (catalogue number)
        /// </summary>
        public string ItemCode { get; set; }

        /// <summary>
        /// Description of product
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Brand of product
        /// </summary>
        public string Brand { get; set; }

        /// <summary>
        /// Model number
        /// </summary>
        public string ModelNum { get; set; }

        /// <summary>
        /// Notes for product
        /// </summary>
        public string Notes { get; set; }

        /// <summary>
        /// Image source of product
        /// </summary>
        public string ImageFileName { get; set; }

        /// <summary>
        /// Get MFR of selected product
        /// </summary>
        public string MFR { get; set; }

        /// <summary>
        /// Soft Id for questions
        /// </summary>
        public int SoftId { get; set; }

        /// <summary>
        /// Enterprise model category
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// Get Id of selected model
        /// </summary>
        public int ModelId { get; set; }

        /// <summary>
        /// Get ShowCallCentreRepair flag
        /// </summary>
        public bool ShowCallCentreRepair { get; set; }

        /// <summary>
        /// Get flag is Free Spares button ENABLED
        /// </summary>
        public bool PartAvailable { get; set; }

        /// <summary>
        /// Repair FAQ
        /// </summary>
        public string RepairFaq { get; set; }
        
        /// <summary>
        /// Repair FAQ for CC
        /// </summary>
        public string CCRepairFaq { get; set; }

        /// <summary>
        /// Is product have alternative product list
        /// </summary>
        public bool AlternativeFlag { get; set; }

        /// <summary>
        /// Process id of Book Repair
        /// </summary>
        public int? BookRepairProcessId { get; set; }
        
        /// <summary>
        /// Get the mobile number for notification
        /// </summary>
        public string NotificationMobileNumber { get; set; }

        /// <summary>
        /// Get email address for notification
        /// </summary>
        public string NotificationEmailAddress { get; set; }

        /// <summary>
        /// Is call center
        /// </summary>
        public bool IsCallCenter { get; set; }

        /// <summary>
        /// Product services
        /// </summary>
        public ProductServicesModel ProductServices { get; set; }

        /// <summary>
        /// Additional links list
        /// </summary>
        public List<Product_AdditServiceModel> AdditioalLinks { get; set; }
        /// <summary>
        /// Manualid
        /// </summary>
        public string Model { get; set; }

        public string ModelBrand { get; set; }

        public int RepairID { get; set; }
    }
}