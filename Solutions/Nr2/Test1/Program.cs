using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test1
{
    class Program
    {
        static void Main(string[] args)
        {
            uk.co.fieldengineer.litmus.TOnlineBookResponseDetails response = BookJob();

            string result = string.Format("BookSuccessfully: {0}\r\nErrorCode: {1}\r\nErrorMsg: {2}\r\nServiceID: {3}\r\nCustomerID: {4}\r\nCustomerName: {5}\r\nDiaryID: {6}\r\nFraudCheckCode: {7}\r\n",
                response.BookSuccessfully.ToString(), response.ErrorCode.ToString(), response.ErrorMsg, response.ServiceID.ToString(), response.CustomerID.ToString(), response.CustomerName, response.DiaryID.ToString(), response.FraudCheckCode.ToString());

            Console.Write(result);

            /*
            List<BookOptionResult> result = GetBookOptionResults();

            foreach (var r in result)
            {
                Console.Write(r.ToString() + "\r\n\r\n");
            } */
        }

        public static List<BookOptionResult> GetBookOptionResults()
        {
            List<BookOptionResult> result = new List<BookOptionResult>();

            uk.co.fieldengineer.litmus.IFzOnlineBookingservice onlineClient = new uk.co.fieldengineer.litmus.IFzOnlineBookingservice();

            uk.co.fieldengineer.litmus.TRequestDetails Request = new uk.co.fieldengineer.litmus.TRequestDetails();

            Request.SaediID = "SONY3C";
            Request.ClientID = 610;
            Request.ClientPassword = "SONCAIR43";
            Request.RequestedDate = "25/05/2019";
            Request.BookingOptions = 5;
            Request.Postcode = "KT6 7EH";
            Request.AddressLine1 = "Gilfach Manafon";
            Request.ApplianceCode = "SONYC";
            Request.UniqueDates = true;

            uk.co.fieldengineer.litmus.TResponseDetails response = onlineClient.AppointmentRequest(Request);

            if (response.RequestSuccess)
            {
                foreach (var item in response.BookingOptionResult)
                {
                    BookOptionResult bookitem = new BookOptionResult();
                    bookitem.Calls = item.Calls;
                    bookitem.Description = item.Description;
                    bookitem.EngineerID = item.EngineerID;
                    bookitem.EngineerName = item.EngineerName;
                    bookitem.EventDate = item.EventDate;
                    result.Add(bookitem);
                }
            }

            return result;
        }

        public static uk.co.fieldengineer.litmus.TOnlineBookResponseDetails BookJob()
        {
            uk.co.fieldengineer.litmus.TOnlineBookRequestDetails request = new uk.co.fieldengineer.litmus.TOnlineBookRequestDetails();
            request.CustomerID = 2683817;
            request.CustAplID = 4117263;
            request.Postcode = "KT6 7EH";
            request.ApplianceCD = "SONYC";
            request.MFR = "N/A";
            request.Model = "SONYCOLLECTION";
            request.EngineerID = 1000;
            request.VisitDate = new DateTime(2019, 6, 3);
            request.PolicyNumber = "PL56TI";
            request.ClientID = 610;
            request.ClientRef = "1076";
            request.ReportFault = "Sony RMA Collection OL56TI 23343545 SON Sony From : LCC LCD TV > 30 UP TO 45 Serial No:";

            uk.co.fieldengineer.litmus.IFzOnlineBookingservice onlineClient = new uk.co.fieldengineer.litmus.IFzOnlineBookingservice();

            return onlineClient.BookNow(request);
        }

        public static void Test1()
        {
            int[] list = new int[5] { 1, 2, 3, 4, 5 };

            var aaa = list.Where(x => x % 2 == 0).OfType<int>();

            foreach (var a in aaa)
            {
                Console.WriteLine(a);
            }        
        }
    }

    public class BookOptionResult
    {
        public int Calls { get; set; }
        public string Description { get; set; }
        public int EngineerID { get; set; }
        public string EngineerName { get; set; }
        public DateTime EventDate { get; set; }

        public override string ToString()
        {
            return string.Format("Calls: {0}\r\nDescription: {1}\r\nEngineerID: {2}\r\nEngineerName: {3}\r\nEventDate: {4}", Calls.ToString(), Description, EngineerID.ToString(), EngineerName, EventDate.ToString("yyyy-MM-dd"));
        }
    }
}
