using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using CAST.Validation;
using CAST.ViewModels.Inspection;
using System.Collections.Generic;

namespace CAST.ViewModels.BookRepair
{
    public class BookRepair_JobModel
    {
        public BookRepair_JobModel()
        {
            StatusID = 4;
            appointmentModel = new AppointmentModel();
        }

        [StringLength(30, ErrorMessage = "Maximum 30 characters")]
        public string SerialNumber { get; set; }

        public string ItemCondition { get; set; }

        public int Day { get; set; }

        public int Month { get; set; }

        public int Year { get; set; }
        
        public System.DateTime? DateOfPurchase { get; set; }

        public string StoreNumber { get; set; }

        [StringLength(20, ErrorMessage = "Maximum 20 characters")]
        public string TillNumber { get; set; }

        [StringLength(30, ErrorMessage = "Maximum 30 characters")]
        public string TransNumber { get; set; }

        //public SelectList LocationType { get; set; }
        public SelectList Type { get; set; }

        [Required(ErrorMessage = "Select type")]
        public string SelectedType { get; set; }

        [Required (ErrorMessage = "Input description")]
        public string FaultDescr { get; set; }

        public string UserID { get; set; }

        public int? EngineerId { get; set; }

        [Required(ErrorMessage = "Input date")]
        [DateOfPurchaseValidation]
        public string DateOfPurchaseString { get; set; }

        public int StatusID { get; set; }

        //public Inspection_Model AdditionalFields { get; set; }
          public List<Inspection_Result> AdditionalFields { get; set; }
          public AppointmentModel appointmentModel { get; set; }
      //  Inspection_Result

          public string AppointmentDate { get; set; }
          public bool StoreCollection { get; set; }
          public string strStoreCollection { get; set; }
          public List<SelectListItem> StoreCollectionAnswerList { get; set; }
          public int Slotid { get; set; }
          public bool OnlineBookingFailed { get; set; }
          public string BookingUrl { get; set; }
          public bool InHomeAvailable { get; set; }
    }
}