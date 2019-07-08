using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mobile.Portal.Classes;
using Mobile.Portal.DAL;
using System.Configuration;
using Mobile.Portal.Services.Onlineservice;
using Mobile.Portal.Helpers;
using Mobile.Portal.Services;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Net;
using System.IO;
using System.Globalization;
using System.Web;
using System.Net.Http;
using System.ServiceModel.Description;
//using Mobile.Portal.Services.ServiceReference1;

namespace Mobile.Portal.BLL
{
    public class CourierRMABLL : BaseBLL<CourierRMA>, Mobile.Portal.BLL.ICourierRMABLL
    {
        ICourierRMADataProvider _dal;
        OnlineBookingLogBLL onlineBookingLogBLL;
        //  FzOnlineBookingClient onlineClient; 
        ClientDataProvider Clientdal;
        IFzOnlineBookingservice onlineClient;
        //   CourierRMAService onlineClient;


        //AuthHeader header;
        Client Saediclient;
        public CourierRMABLL()
        {
            _dal = new CourierRMADataProvider();
            Clientdal = new ClientDataProvider();
            onlineClient = new IFzOnlineBookingservice();
            //  onlineClient = new CourierRMAService();
            // The HTTP request is unauthorized with client authentication scheme 'Anonymous'. The authentication header received from the server was 'Basic'.
            //onlineClient.ClientCredentials.UserName.Password = "SONCAIR432";
            //onlineClient.ClientCredentials.UserName.UserName = "SONY3C";

            // onlineClient.Credentials = new ClientCredentials(

            //System.Net.CredentialCache sysCredentail = new System.Net.CredentialCache();
            //NetworkCredential netCred = new NetworkCredential("SONY3C", "SONCAIR432");
            //sysCredentail.Add(new Uri(strSysURL), "Basic", netCred);
            //onlineClient.Credentials = sysCredentail;
            //  ascService = new IFzOnlineBooking();
            //ascService.ClientCredentials
            onlineBookingLogBLL = new OnlineBookingLogBLL();
            //header = new AuthHeader();
            Saediclient = new Client();
        }

        public List<BookOptionResult> AppointmentRequest(string RMAid, string saediFromid)
        {
            List<BookOptionResult> result = new List<BookOptionResult>();
            RMARef RMAresult = new RMARef();
            SwapforCredit swapresult = new SwapforCredit();
            RMARefDataProvider dal = new RMARefDataProvider();
            Swap2CreditDataProvider swapdal = new Swap2CreditDataProvider();

            try
            {
                RMAresult = dal.GetPartsRMADetailsbyRMAid(RMAid, saediFromid);
                ErrorHandler.LogToFile(String.Format("{0} :{1} :Starting Appointment request for {2}", saediFromid, RMAresult.ClientRef, RMAresult.rmaId));
            }
            catch
            {
                swapresult = swapdal.GetSWAPRMADetailsbyRMAid(RMAid, saediFromid);
                ErrorHandler.LogToFile(String.Format("{0} :{1} :Starting Appointment request for {2}", saediFromid, swapresult.ClientRef, swapresult.rmaId));
            }


            Saediclient = Clientdal.GetBySaediId(saediFromid);
            Saediclient.onlinebookingSetting = Clientdal.FetchOnlinebookingSetting(Saediclient.OSPRef);
            onlinebookingStting(Saediclient);

            //header.Username = Saediclient.OSPRef;
            //header.Password = Saediclient.onlinebookingSetting.SaediPassword;
            try
            {
                ErrorHandler.LogToFile(String.Format("{0} :{1} :fetching client details :customerid {2} custaplid {3}", saediFromid, RMAresult.ClientRef, Saediclient.OnlineCustomerId, Saediclient.CustomeraplId));
            }
            catch
            {
                ErrorHandler.LogToFile(String.Format("{0} :{1} :fetching client details :customerid {2} custaplid {3}", saediFromid, swapresult.ClientRef, Saediclient.OnlineCustomerId, Saediclient.CustomeraplId));

            }
            try
            {

                //Add or update engineer address
                int CustomerId = AddCustomer(Saediclient.onlinebookingSetting.OnlineBookingURL);
                if (CustomerId == 0)
                    throw new Exception("Add customer Failure");

                //check CustAPLid exists or not // if appln not exists add it
                if (Saediclient.CustomeraplId == 0)
                {
                    Saediclient.OnlineCustomerId = CustomerId;
                    int CustAplid = AddCustApl(Saediclient.onlinebookingSetting.OnlineBookingURL);

                    Saediclient.CustomeraplId = CustAplid;
                    Clientdal.UpdateClientCollectionBookingInfo(Saediclient);
                }
                // fetching Appointments
                if (Saediclient.CustomeraplId != 0)
                {
                    TResponseDetails appointments = FetchAppointments();

                    if (appointments.RequestSuccess)
                    {
                        foreach (var item in appointments.BookingOptionResult)
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
                }
                return result;

            }
            catch (Exception ex)
            {
                string error = string.Format("Error on booking Courier collection for RMA :{0} . Please do it later.", RMAid);
                return result;
            }

        }

        private void onlinebookingStting(Client Saediclient)
        {

            ErrorHandler.LogToFile(String.Format("onlinebookingStting{0} {1} {2} ", Saediclient.onlinebookingSetting.OnlineBookingURL, Saediclient.OSPRef, Saediclient.onlinebookingSetting.SaediPassword));

            onlineClient.Url = Saediclient.onlinebookingSetting.OnlineBookingURL;//@"http://10.10.10.35/FZOnlineBooking-JTM/FZOnlineBooking.dll/soap/IFzOnlineBooking";
            onlineClient.PreAuthenticate = true;
            onlineClient.Credentials = new System.Net.NetworkCredential(Saediclient.OSPRef, Saediclient.onlinebookingSetting.SaediPassword);
            ErrorHandler.LogToFile(String.Format("onlinebooking Setting {0} {1} {2} ", Saediclient.onlinebookingSetting.OnlineBookingURL, Saediclient.OSPRef, Saediclient.onlinebookingSetting.SaediPassword));



        }

        private TResponseDetails FetchAppointments()
        {
            var Request = AppointmentMapping();
            ErrorHandler.LogToFile("{0} :{1} :Starting Appointment request");

            var response = onlineClient.AppointmentRequest(Request);
            onlineBookingLogBLL.InsertOnlineServiceLog(Request, response, Saediclient.SaediId, Saediclient.onlinebookingSetting.OnlineBookingURL, response.RequestSuccess);
            return response;

        }

        private TRequestDetails AppointmentMapping()
        {
            TRequestDetails Request = new TRequestDetails();
            Request.SaediID = Saediclient.OSPRef;
            Request.ClientID = Saediclient.onlinebookingSetting.Clientid;

            Request.ClientPassword = Saediclient.onlinebookingSetting.SaediPassword;
            Request.RequestedDate = String.Format("{0:dd/MM/yyyy}", DateTime.Now.AddDays(int.Parse(ConfigurationManager.AppSettings["D+Days"].ToString())));
            Request.BookingOptions = 5;
            Request.Postcode = Saediclient.PostalCode;
            Request.AddressLine1 = Saediclient.DeliveryAddress.Address1;
            Request.ApplianceCode = Saediclient.onlinebookingSetting.ApplianceCD;
            Request.UniqueDates = true;


            return Request;
        }

        public int AddCustomer(string url)
        {
            int Customerid = 0;
            TCustomer customer = CustomerMapping();
            TCustomer result = new TCustomer();
            try
            {
                ErrorHandler.LogToFile(String.Format("{0} : Creating/ Updating customer details ", Saediclient.SaediId));


                onlinebookingStting(Saediclient);


                result = onlineClient.AddCustomer(customer);
                ErrorHandler.LogToFile(String.Format("adding customer{0} {1} {2} ", result.CustomerID, result.ErrorCode, result.ErrorMsg));

                onlineBookingLogBLL.InsertOnlineServiceLog(customer, result, Saediclient.SaediId, Saediclient.onlinebookingSetting.OnlineBookingURL, result.ErrorCode == 0); //todo: url from ospref
                Customerid = result.CustomerID;
                ErrorHandler.LogToFile(String.Format("{0} : Created/ Updated customer details ; customer id : {1} ", Saediclient.SaediId, Customerid));
                return Customerid;
            }
            catch (Exception ex)
            {
                ErrorHandler.LogToFile(String.Format("error on adding customer{0} {1} {2} ", result.CustomerID, result.ErrorCode, ex.Message));

                onlineBookingLogBLL.InsertOnlineServiceLog(customer, ex.Message, Saediclient.SaediId, Saediclient.onlinebookingSetting.OnlineBookingURL, false);
                return Customerid;
            }

        }

        private TCustomer CustomerMapping()
        {
            TCustomer customer = new TCustomer();
            customer.CustomerID = Saediclient.OnlineCustomerId;
            customer.ClientCustRef = Saediclient.SaediId;
            // customer.Title= Saediclient.
            customer.Surname = Saediclient.CompanyName == "" ? "." : Saediclient.CompanyName;
            customer.Firstname = Saediclient.DeliveryAddress.Contact == "" ? "." : Saediclient.DeliveryAddress.Contact;
            customer.Addr1 = Saediclient.DeliveryAddress.Address1;
            customer.Addr2 = Saediclient.DeliveryAddress.Address2;
            customer.Addr3 = Saediclient.DeliveryAddress.Address3;
            customer.Postcode = Saediclient.PostalCode;
            customer.Town = Saediclient.DeliveryAddress.Address4;
            customer.County = Saediclient.DeliveryAddress.Address5;
            customer.Telephone = Saediclient.TelNo;
            //customer.Telephone2=
            //customer.Fax=
            customer.Email = Saediclient.EmailAddress;
            customer.ClientID = Saediclient.onlinebookingSetting.Clientid;
            //  customer.Telephone3=
            // customer.PreferredContactMethod
            return customer;
        }


        private TCustomerAppliance CustomerApplianceMapping()
        {
            TCustomerAppliance customerAppliance = new TCustomerAppliance();
            customerAppliance.CustomerID = Saediclient.OnlineCustomerId;
            customerAppliance.ApplianceCD = Saediclient.onlinebookingSetting.ApplianceCD;
            customerAppliance.Model = Saediclient.onlinebookingSetting.Model;
            customerAppliance.MFR = Saediclient.onlinebookingSetting.Manufacture;

            return customerAppliance;
        }

        public int AddCustApl(string url)
        {
            int custaplid = 0;
            ErrorHandler.LogToFile(String.Format("{0}  : Adding  customer appliance details ", Saediclient.SaediId));
            TCustomerAppliance appliance = CustomerApplianceMapping();
            TCustomerAppliance result = new TCustomerAppliance();
            try
            {
                ErrorHandler.LogToFile(String.Format("{0} : Creating/ Updating customer appliance details ", Saediclient.SaediId));
                //onlineClient.Endpoint.Address = new EndpointAddress(new Uri(Saediclient.onlinebookingSetting.OnlineBookingURL));
                result = onlineClient.AddCustomerAppliance(appliance);
                // result = onlineClient.AddCustomerAppliance(ref header, appliance);
                onlineBookingLogBLL.InsertOnlineServiceLog(appliance, result, Saediclient.SaediId, Saediclient.onlinebookingSetting.OnlineBookingURL, true); //todo: url from ospref
                custaplid = result.CustAplID;
                ErrorHandler.LogToFile(String.Format("{0} : Created/ Updated customer details ; customer appliance id : {1} ", Saediclient.SaediId, custaplid));
                return custaplid;
            }
            catch (Exception ex)
            {
                onlineBookingLogBLL.InsertOnlineServiceLog(appliance, result, Saediclient.SaediId, Saediclient.onlinebookingSetting.OnlineBookingURL, false);
                return custaplid;
            }

        }
        public RetriveMediaResponse RetreiveMedia(string url, string linkid, int TypeId, int MediaContext, string saediFromid)
        {
            RetriveMediaResponse media = new RetriveMediaResponse();

            try
            {
                media = RetreiveMediaMapping(url, linkid, TypeId, MediaContext, saediFromid);

            }
            catch (Exception ex)
            {
                onlineBookingLogBLL.InsertOnlineServiceLog("RetreiveMedia", ex.Message.ToString(), saediFromid, url, false);

            }

            return media;


        }

        private RetriveMediaResponse RetreiveMediaMapping(string url, string linkid, int TypeId, int MediaContext, string saediFromid)
        {

            RetriveMediaResponse mediaresult = new RetriveMediaResponse();
            onlineClient.Url = url;
            TMediaResponse mediaResponse = onlineClient.RetrieveMedia(Convert.ToInt32(linkid), TypeId, MediaContext);
            //mapping media
            onlineBookingLogBLL.InsertOnlineServiceLog(mediaResponse, mediaResponse, saediFromid, url, mediaResponse.ErrorCode != 0);

            if (mediaResponse.ErrorCode == 0)
            {
                foreach (TMedia item in mediaResponse.MediaList)
                {
                    ReceivedMedia receivedMedia = new ReceivedMedia();
                    receivedMedia.LinkID = item.LinkID;
                    receivedMedia.TypeID = item.TypeID;
                    receivedMedia.MediaExtension = item.MediaExtension;
                    receivedMedia.MediaText = item.MediaText;
                    receivedMedia.MediaTimeStamp = item.MediaTimeStamp;
                    receivedMedia.MediaSubject = item.MediaSubject;
                    receivedMedia.MediaContext = item.MediaContext;
                    receivedMedia.MediaPrivateFg = item.MediaPrivateFg;
                    receivedMedia.MSGUID = item.MSGUID;
                    receivedMedia.FileName = item.FileName;

                    mediaresult.receivedMedia.Add(receivedMedia);
                }

            }
            else
            {
                mediaresult.ErrorCode = mediaresult.ErrorCode;
                mediaresult.ErrorText = mediaresult.ErrorText;
            }

            return mediaresult;

        }


        public int BookJob(string SelectedDate, int SelectedEngineerid, string saediFromid, string RMAID, Call call, bool SwapclaimCollection = false)
        {
            TOnlineBookResponseDetails response = new TOnlineBookResponseDetails();
            ClientDataProvider Clientdal = new ClientDataProvider();
            Saediclient = Clientdal.GetBySaediId(saediFromid);
            Saediclient.onlinebookingSetting = Clientdal.FetchOnlinebookingSetting(Saediclient.OSPRef);
            onlinebookingStting(Saediclient);
            TOnlineBookRequestDetails request = new TOnlineBookRequestDetails();
            // request = BookJobMapping(SelectedDate, SelectedEngineerid, saediFromid, RMAID);
            request = BookJobMapping(SelectedDate, SelectedEngineerid, call, RMAID, Saediclient.onlinebookingSetting.Clientid, SwapclaimCollection);
            try
            {
                //onlineClient.Endpoint.Address = new EndpointAddress(new Uri(Saediclient.onlinebookingSetting.OnlineBookingURL));
                response = onlineClient.BookNow(request);
                onlineBookingLogBLL.InsertOnlineServiceLog(request, response, Saediclient.SaediId, Saediclient.onlinebookingSetting.OnlineBookingURL, response.BookSuccessfully); //todo: url from ospref
                return response.ServiceID;

            }
            catch (Exception ex)
            {
                onlineBookingLogBLL.InsertOnlineServiceLog(request, ex.Message + " " + response, Saediclient.SaediId, Saediclient.onlinebookingSetting.OnlineBookingURL, response.BookSuccessfully); //todo: url from ospref
                return 0;
            }
        }

        private TOnlineBookRequestDetails BookJobMapping(string SelectedDate, int SelectedEngineerid, Call call, string RMAID, int Clientid, bool SwapClaimCollection = false)
        {
            Swap2CreditDataProvider swapdal = new Swap2CreditDataProvider();
            SwapforCredit swap2Credit = new SwapforCredit();

            RMARefDataProvider dal = new RMARefDataProvider();
            RMARef RMAresult = new RMARef();

            if (!SwapClaimCollection)
                RMAresult = dal.GetPartsRMADetailsbyRMAid(RMAID, call.SaediFromId);
            else
                swap2Credit = swapdal.GetSWAPRMADetailsbyRMAid(RMAID, call.SaediFromId);

            TOnlineBookRequestDetails request = new TOnlineBookRequestDetails();
            request.CustomerID = Saediclient.OnlineCustomerId;
            request.CustAplID = Saediclient.CustomeraplId;
            request.Postcode = Saediclient.PostalCode;
            request.ApplianceCD = Saediclient.onlinebookingSetting.ApplianceCD;
            request.MFR = Saediclient.onlinebookingSetting.Manufacture;
            request.Model = Saediclient.onlinebookingSetting.Model;
            request.EngineerID = SelectedEngineerid;
            request.VisitDate = DateTime.Parse(SelectedDate);//, "yyyy/MM/dd", CultureInfo.InvariantCulture);
            request.PolicyNumber = RMAID;
            request.ClientID = Clientid;
            request.ClientRef = call.ClientRef;
            if (!SwapClaimCollection)
                request.ReportFault = string.Format("Sony RMA Collection {1} {0}{7} {8}{0}{2}{5}{0}From :{3}  {4}  {0}Serial No:{6}", Environment.NewLine, RMAID, RMAresult.INPUT_partNumberReceived, RMAresult.INPUT_modelID,
                    call.Appliance.ApplianceType, call.Appliance.Manufacturer, RMAresult.INPUT_sonNumber, RMAresult.INPUT_serialNumber, RMAresult.INPUT_PartNumber, RMAresult.Partdesc);
            else
                request.ReportFault = string.Format("Sony RMA Collection {1} {0}{0}{2}{5}{0}From :{3}  {4}  {0}Serial No:{6}", Environment.NewLine, RMAID, swap2Credit.INPUT_modelID,
             call.Appliance.ApplianceType, call.Appliance.Manufacturer, swap2Credit.INPUT_sonNumber, swap2Credit.INPUT_serialNumber);
            return request;
        }



        internal AddMediaResponse AddMedia(string url, string linkid, int TypeId, int MediaContext, string saediFromid,MediaUpload mediaresult)
        {
            AddMediaResponse media = new AddMediaResponse();

            try
            {
                media = AddMediaMapping(url, linkid, TypeId, MediaContext, saediFromid, mediaresult);

            }
            catch (Exception ex)
            {
                onlineBookingLogBLL.InsertOnlineServiceLog("AddMedia info to cs", ex.Message.ToString(), saediFromid, url, false);

            }

            return media;
        }

        private AddMediaResponse AddMediaMapping(string url, string linkid, int TypeId, int MediaContext, string saediFromid,MediaUpload mediaresult)
        {
            AddMediaResponse mediaresponse = new AddMediaResponse();
            onlineClient.Url = url;
            TMedia tmedia = new TMedia();
            tmedia.MSGUID= mediaresult.mediauploadResponse.MSGUID;
            tmedia.TypeID= mediaresult.mediauploadRequest.TypeID;
            tmedia.MediaTimeStamp= mediaresult.mediauploadRequest.TimeStamp;
            tmedia.MediaText= mediaresult.mediauploadRequest.Notes;
            tmedia.MediaSubject=mediaresult.mediauploadRequest.Notes;
            tmedia.MediaPrivateFg=mediaresult.mediauploadRequest.MediaPrivateFg;
            tmedia.MediaExtension=mediaresult.mediauploadRequest.MediaExtension;
            tmedia.MediaContext=mediaresult.mediauploadRequest.ContextID;
            tmedia.FileName=mediaresult.mediauploadRequest.FileName;
            tmedia.LinkID=mediaresult.mediauploadRequest.LinkId;


            TAddMediaResponse mediaResponse = onlineClient.AddMedia(tmedia);
            //mapping media
            onlineBookingLogBLL.InsertOnlineServiceLog(tmedia, mediaResponse, saediFromid, url, mediaResponse.ErrorCode != 0);


            mediaresponse.ErrorCode = mediaResponse.ErrorCode;
            mediaresponse.ErrorText= mediaResponse.ErrorText;



            return mediaresponse;
        }

        public void AddMediaMapping(string url, string guid, bool mediaPrivateFg, string mediaExtension, string notes, int linkId, int typeId, int mediaContext, string saediFromid)
        {
            onlineClient.Url = url;
            TMedia tmedia = new TMedia();
            tmedia.MSGUID = guid;
            tmedia.TypeID = typeId;
            tmedia.MediaTimeStamp = DateTime.Now.ToString();
            tmedia.MediaText = notes;
            tmedia.MediaSubject = notes;
            tmedia.MediaPrivateFg = mediaPrivateFg;
            tmedia.MediaExtension = mediaExtension;
            tmedia.MediaContext = mediaContext;
            tmedia.FileName = string.Format("{0}_{1}.{2}", saediFromid, DateTime.Now.ToString("yyyyMMddHHmmssfff"), mediaExtension);
            tmedia.LinkID = linkId;
            onlineClient.AddMedia(tmedia);
        }
    }
}
