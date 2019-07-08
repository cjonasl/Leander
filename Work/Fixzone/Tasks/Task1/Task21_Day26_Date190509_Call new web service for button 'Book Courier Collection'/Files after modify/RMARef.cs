using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mobile.Portal.Classes
{
    public class RMARef
    {
        public string SaediFromId { get; set; }
        public string ClientRef { get; set; }
        public bool success { get; set; }
        public string errorMessage { get; set; }
        public string[] validationErrorList { get; set; }
        public string rmaId { get; set; }
        public string validationStatus { get; set; }
        public string shipmentStatus { get; set; }
        public string rmaDocumentUrl { get; set; }
        public string statusID { get; set; }

        public string INPUT_Remark { get; set; }
        public string INPUT_ClaimType { get; set; }
        public string INPUT_GpToolRmaId { get; set; }
        public string INPUT_PCB { get; set; }
        public string INPUT_AepBookinRef { get; set; }
        public string INPUT_FaultCode { get; set; }
        public string INPUT_PartNumber { get; set; }
        public string INPUT_WarranytStatus { get; set; }
        public string INPUT_externalMatId { get; set; }
        public string INPUT_clientReference { get; set; }
        public string INPUT_faultCode { get; set; }
        public string INPUT_repairNumber { get; set; }
        public string INPUT_modelID { get; set; }
        public string INPUT_returnQuantity { get; set; }
        public string INPUT_serialNumber { get; set; }
        public string INPUT_sonNumber { get; set; }
        public string INPUT_partNumberReceived { get; set; }
        public string Collectionref { get; set; }
        public string CollectionDate { get; set; }
        public string  CollectionAddedOn { get; set; }
        public string Partdesc { get; set; }
        public string ShipmateMediaURL { get; set; }

        public class RmaResponseForView
        {
            public string StockCode { get; set; }
            public bool IsError { get; set; }
            public bool IsStockPart { get; set; }
            public string Success { get; set; }
            public string ErrorMessage { get; set; }
            public string ErrorList { get; set; }
            public string ValidationStatus { get; set; }
            public string ShipmentStatus { get; set; }
            public string DocumentURL { get; set; }
            public string Rma { get; set; } 
        
        }
    }
}

