using ClientConnect.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ClientConnect.ViewModels.BookNewService
{
    [Serializable]
    public class AppointmentModel
    {
        public AppointmentModel()
        {
            //availabiltyModel = new AvailableRequestDetails();
            //availabiltyResponseModel = new AvailableResponseDetails();
            availabiltyModel = new RequestDetails();
            availabiltyResponseModel = new ResponseDetails();
         //   Visitcodes =  new List<SelectListItem>();

     }
        public DateTime? PreferredVisitDate { get; set; }
      
        public bool IsGetAvailabiltyInfoPressed { get; set; }  
        public string TroubleShootDescr { get; set; }
        public string FaultDescr { get; set; }
        public RequestDetails availabiltyModel { get; set; }

        public ResponseDetails availabiltyResponseModel { get; set; }
        //public AvailableRequestDetails availabiltyModel { get; set; }

        //public AvailableResponseDetails availabiltyResponseModel { get; set; }

        public string Visitcode { get; set; }
        public List<SelectListItem> Visitcodes { get; set; }
    }
}