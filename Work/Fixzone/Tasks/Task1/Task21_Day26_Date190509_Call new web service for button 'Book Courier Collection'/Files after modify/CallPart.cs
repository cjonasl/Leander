using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mobile.Portal.Classes
{    
    [Serializable]
    public class CallPart : TransactionItem
    {
        public CallPart()
        {
            RmaDetails = new List<RMARef>();
        }

        public CallPart(int lineNo, string code, string description, int quantity, decimal unitPrice, string taxId,
            decimal taxRate, DateTime? orderDate, DateTime? dispatchDate, string deliveryNumber, string transactionCode,
            string status, int partReference, bool isFitted, bool isEstimate, bool isStock, bool usePriceCheck)
        {
            LineNo = lineNo;
            Code = code;
            Description = description;
            Quantity = quantity;
            UnitPrice = unitPrice;
            TaxId = taxId;
            TaxRate = taxRate;
            OrderDate = orderDate;
            DispatchDate = dispatchDate;
            DeliveryNumber = deliveryNumber;
            OrderDate = orderDate;
            DispatchDate = dispatchDate;
            DeliveryNumber = deliveryNumber;
            TransactionCode = transactionCode;
            Status = status;
            PartReference = partReference;
            IsFitted = isFitted;
            IsEstimate = isEstimate;
            IsStock = isStock;
            UsePriceCheck = usePriceCheck;
        }

        public string SAEDIFromID { get; set; }
        public string SAEDICallRef { get; set; }
        public DateTime? OrderDate { get; set; }
        public DateTime? DispatchDate { get; set; }
        public string DeliveryNumber { get; set; }
        public string CourierReference { get; set; }
        public int PartReference { get; set; }
        public string OrderReference { get; set; }
        public string Status { get; set; }
        public string StatusID { get; set; }
        public string FigNo { get; set; }
        public bool IsFitted { get; set; }
        public bool IsEstimate { get; set; }
        public bool IsStock { get; set; }
        public bool IsPrimary { get; set; }
        public bool IsPartConsumptionDone { get; set; }
        // public bool IsRMADone { get; set; }
        public PartAction Action { get; set; }
        public bool UsePriceCheck { get; set; }
        public string Available { get; set; }
        public string ReturnDescription { get; set; }
        public string ReturnReference { get; set; }
        public bool ReturnRequired { get; set; }
        public string RmaDocumentUrl { get; set; } 
        public string StockType { get; set; }
        public string ErrorMessage { get; set; }
        public string PartNote { get; set; }
        public int VanQuantInStock { get; set; } 

        // FOR SONY:
        public bool IsSony { get; set; }
        public string INPUTascMaterialId { get; set; }
        public bool IsAllocated { get; set; }

        //RMA collection properties
        public bool NeedCollection
        {
            get
            {
                if (!string.IsNullOrEmpty(RmaDocumentUrl))// && string.IsNullOrEmpty(Collectionref) && (Collectionref.Trim().Length==0))
                    return string.IsNullOrEmpty(Collectionref) ? true : Collectionref.Trim().Length == 0;

                    //  return true;
                else
                    return false;
            }         

        }
        public string Collectionref { get; set; }
        public string CollectionDate { get; set; }
        public bool IsBulletin { get; set; }
        public bool IsRmaDone { get; set; }
        public string RmaErrorDescription { get; set; }                 
        public string INPUTson { get; set; }   
        public string SonyPartStatus {
            get {
                if (IsBulletin)
                    return "Bulletin Part";
                if (Code == "000000010")
                    return "NIP Part";
                if (StatusID == "V")
                    return "Stock Part";
                if (IsAllocated)
                    return "Allocated Part";
                else
                    return string.Empty;
            }         
        }
        public string ValidationStatus { get; set; }
        public string ShipmentStatus { get; set; }
        public string WarrantyStatus { get; set; }
        public string LabelUrl {get; set;}
        public string INPUT_CourierID { get; set; }
        public string ConNoteUrl { get; set; }
        public string RetCourierRef
        { get; set; }
        public string ConsignmentNo
        { get; set; }
        public string BookingUniqueNumber
        { get; set; }
        public List<RMARef> RmaDetails { get; set; }
        public string ShipmateMediaURL { get; set; }
     
        //private string _partNote = string.Empty;
        
        //{
        //    get
        //    {
        //        return _partNote;
        //    }
        //    set
        //    {
        //        if (value == null)
        //            _partNote = string.Empty;
        //        else
        //            _partNote = value.Substring(0, Math.Min(20, value.Length));
        //    }
        //}

       

    }
    //[Serializable]
    //public class RMAdetail
    //{
    //    public int LineNo { get; set; }
    //    public string ReturnReference { get; set; }
    //    public bool ReturnRequired { get; set; }
    //    public string RmaDocumentUrl { get; set; } 
    //}
}
