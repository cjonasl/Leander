using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Text;
using System.Net;
using System.Web.Script.Serialization;
using Mobile.Portal.Helpers;
using Mobile.Portal.DAL;

namespace Mobile.Portal.BLL.Shipmate
{
    public enum ShipmateAction
    {
        Login,
        Services,
        CreateConsignment,
        TrackingByConsignment,
        TrackingByParcels,
        CancelConsignment,
        Label,
        PrintLabel
    }

    public class NameValueType
    {
        public string Type { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }

        public NameValueType(string type, string name, string value)
        {
            this.Type = type;
            this.Name = name;
            this.Value = value;
        }
    }

    public static class Utility
    {
        public static string Write(this string str)
        {
            if (str == null)
                return "NULL";
            else if (str.Trim() == string.Empty)
                return "Empty string";
            else
                return str;
        }

        public static string ReplaceNullAndWhiteSpaceWithEmptyString(this string str)
        {
            if (string.IsNullOrEmpty(str) || string.IsNullOrWhiteSpace(str))
                return "";
            else
                return str;
        }

        public static string PrintListOfStrings(List<string> list)
        {
            StringBuilder sb = new StringBuilder("[");

            for (int i = 0; i < list.Count; i++)
            {
                if (i == 0)
                    sb.Append(list[i]);
                else
                    sb.Append(", " + list[i].Write());
            }

            sb.Append("]");

            return sb.ToString();
        }
    }

    public class User
    {
        public string id { get; set; }
        public string user_type { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string email { get; set; }
        public string account_name { get; set; }
        public string sender_id { get; set; }

        public override string ToString()
        {
            return string.Format("(string) id = {0}\r\n(string) user_type = {1}\r\n(string) first_name = {2}\r\n(string) last_name = {3}\r\n(string) email = {4}\r\n(string) account_name = {5}\r\n(string) sender_id = {6}",
                id.Write(), user_type.Write(), first_name.Write(), last_name.Write(), email.Write(), account_name.Write(), sender_id.Write());
        }
    }

    public class LoginResponseData
    {
        public string token { get; set; }
        public User user { get; set; }

        public override string ToString()
        {
            return string.Format("(string) token = {0}\r\n(User) user =\r\n{1}", token, user != null ? user.ToString() : "null");
        }
    }

    public class ToAddressRequest
    {
        public string name { get; set; }
        public string line_1 { get; set; }
        public string city { get; set; }
        public string postcode { get; set; }
        public string country { get; set; }

        public ToAddressRequest(string name, string line1, string city, string postcode, string country)
        {
            this.name = name.ReplaceNullAndWhiteSpaceWithEmptyString();
            this.line_1 = line1.ReplaceNullAndWhiteSpaceWithEmptyString();
            this.city = city.ReplaceNullAndWhiteSpaceWithEmptyString();
            this.postcode = postcode.ReplaceNullAndWhiteSpaceWithEmptyString();
            this.country = country.ReplaceNullAndWhiteSpaceWithEmptyString();
        }

        public override string ToString()
        {
            return string.Format("(string) name = {0}\r\n(string) line_1 = {1}\r\n(string) city = {2}\r\n(string) postcode = {3}\r\n(string) country = {4}",
                name.Write(), line_1.Write(), city.Write(), postcode.Write(), country.Write());
        }

        public string ToJson()
        {
            return (new JavaScriptSerializer()).Serialize(this);
        }
    }

    public class ToAddressResponse
    {
        public string delivery_name { get; set; }
        public string line_1 { get; set; }
        public string line_2 { get; set; }
        public string line_3 { get; set; }
        public string city { get; set; }
        public string county { get; set; }
        public string postcode { get; set; }
        public string country { get; set; }

        public override string ToString()
        {
            return string.Format("(string) delivery_name = {0}\r\n(string) line_1 = {1}\r\n(string) line_2 = {2}\r\n(string) line_3 = {3}\r\n(string) city = {4}\r\n(string) county = {5}\r\n(string) postcode = {6}\r\n(string) country = {7}",
                delivery_name.Write(), line_1.Write(), line_2.Write(), line_3.Write(), city.Write(), county.Write(), postcode.Write(), country.Write());
        }
    }

    public class Parcel
    {
        public string reference { get; set; }
        public int weight { get; set; }
        public int width { get; set; }
        public int length { get; set; }
        public int depth { get; set; }

        public Parcel(string reference, int weight, int width, int length, int depth)
        {
            this.reference = reference;
            this.weight = weight;
            this.width = width;
            this.length = length;
            this.depth = depth;
        }

        public override string ToString()
        {
            return string.Format("(string) reference = {0}\r\n(int) weight = {1}\r\n(int) width = {2}\r\n(int) length = {3}\r\n(int) depth = {4}",
                reference.Write(), weight, width, length, depth);
        }

        public string ToJson()
        {
            return (new JavaScriptSerializer()).Serialize(this);
        }
    }

    public class LoginRequest
    {
        public string username { get; set; }
        public string password { get; set; }

        public LoginRequest(string username, string password)
        {
            this.username = username.ReplaceNullAndWhiteSpaceWithEmptyString();
            this.password = password.ReplaceNullAndWhiteSpaceWithEmptyString();
        }

        public string ToJson()
        {
            return (new JavaScriptSerializer()).Serialize(this);
        }
    }

    public class ConsignmentRequest
    {
        public int ServiceID { get; set; }
        public int RemittanceID { get; set; }
        public string consignment_reference { get; set; }
        public string Token { get; set; }
        public string service_key { get; set; }
        public ToAddressRequest to_address { get; set; }
        public List<Parcel> parcels { get; set; }

        public ConsignmentRequest(int serviceID, int remittanceID, string consignmentReference, string token, string serviceKey, ToAddressRequest toAddress, List<Parcel> parcels)
        {
            this.ServiceID = serviceID;
            this.RemittanceID = remittanceID;
            this.consignment_reference = consignmentReference.ReplaceNullAndWhiteSpaceWithEmptyString();
            this.Token = token.ReplaceNullAndWhiteSpaceWithEmptyString();
            this.service_key = serviceKey.ReplaceNullAndWhiteSpaceWithEmptyString();
            this.to_address = toAddress;
            this.parcels = parcels;
        }

        public override string ToString()
        {
            return string.Format("(int) ServiceID = {0}\r\n(int) RemittanceID = {1}\r\n(string) consignment_reference = {2}\r\n(string) Token = {3}\r\n(string) service_key = {4}\r\n(ToAddressRequest) to_address =\r\n{5}\r\n(List<Parcel>) parcels =\r\n{6}",
                ServiceID.ToString(), RemittanceID.ToString(), consignment_reference.Write(), Token.Write(), service_key.Write(), to_address.ToString(), parcels.ToString());
        }

        public string ToJson()
        {
            return (new JavaScriptSerializer()).Serialize(this);
        }
    }

    public class ServicesResponseData
    {
        public string id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string key { get; set; }
        public string carrier { get; set; }
        public List<string> conditions { get; set; }

        public override string ToString()
        {
            return string.Format("(string) id = {0}\r\n(string) name = {1}\r\n(string) description = {2}\r\n(string) key = {3}\r\n(string) carrier = {4}\r\n(List<string>) conditions = {5}",
                id.Write(), name.Write(), description.Write(), key.Write(), carrier.Write(), Utility.PrintListOfStrings(conditions));
        }
    }

    public class CreateConsignmentResponseData
    {
        public string consignment_reference { get; set; }
        public string parcel_reference { get; set; }
        public string carrier { get; set; }
        public string service_name { get; set; }
        public string tracking_reference { get; set; }
        public string created_by { get; set; }
        public string created_with { get; set; }
        public DateTime created_at { get; set; }
        public ToAddressResponse to_address { get; set; }
        public string pdf { get; set; }
        public string zpl { get; set; }
        public string png { get; set; }

        public override string ToString()
        {
            return string.Format("(string) consignment_reference = {0}\r\n(string) parcel_reference = {1}\r\n(string) carrier = {2}\r\n(string) service_name = {3}\r\n(string) tracking_reference = {4}\r\n(string) created_by = {5}\r\n(string) created_with = {6} \r\n(DateTime) created_at = {7} \r\n(ToAddressResponse) to_address =\r\n{8}\r\n(string) pdf = {9}\r\n(string) zpl = {10}\r\n(string) png = {11}",
                consignment_reference.Write(), parcel_reference.Write(), carrier.Write(), service_name.Write(), tracking_reference.Write(), created_by.Write(), created_with.Write(), created_at.ToString(), to_address != null ? to_address.ToString() : "null", pdf.Write(), zpl.Write(), png.Write());
        }
    }

    public class TrackingEvent
    {
        public string code { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public DateTime date { get; set; }
        public string type { get; set; }

        public override string ToString()
        {
            return string.Format("(string) code = {0}\r\n(string) name = {1}\r\n(string) description = {2}\r\n(DateTime) date = {3}\r\n(string) type = {4}",
                code.Write(), name.Write(), description.Write(), date.ToString(), type.Write());
        }
    }

    public class TrackingConsignmentResponseData
    {
        public string sender_reference { get; set; }
        public string courier_tracking_reference { get; set; }
        public List<TrackingEvent> tracking_events { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(string.Format("(string) sender_reference = {0}\r\n(string) courier_tracking_reference = {1}\r\ntracking_events (list of {2} TrackingEvent data) =\r\n",
                sender_reference.Write(), courier_tracking_reference.Write(), (tracking_events == null ? "0" : tracking_events.Count.ToString())));

            if (tracking_events != null)
            {
                for (int i = 1; i <= tracking_events.Count; i++)
                {
                    sb.Append(string.Format("Data {0}:\r\n{1}\r\n\r\n", i.ToString(), tracking_events[i - 1].ToString()));
                }
            }

            return sb.ToString().TrimEnd();
        }
    }

    public class LoginResponse
    {
        public string message { get; set; }
        public LoginResponseData data { get; set; }

        public override string ToString()
        {
            return string.Format("(string) message = {0}\r\n(LoginResponseData) data = \r\n{1}", message.Write(), data != null ? data.ToString() : "null");
        }
    }

    public class ConsignmentResponse
    {
        public string message { get; set; }
        public List<CreateConsignmentResponseData> data { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(string.Format("(string) message = {0}\r\ndata (list of {1} ConsignmentResponseData data) =\r\n", message.Write(), (data == null ? "0" : data.Count.ToString())));

            if (data != null)
            {
                for (int i = 1; i <= data.Count; i++)
                {
                    sb.Append(string.Format("Data {0}:\r\n{1}\r\n\r\n", i.ToString(), data[i - 1].ToString()));
                }
            }

            return sb.ToString().TrimEnd();
        }
    }

    public class TrackingConsignmentResponse
    {
        public string message { get; set; }
        public TrackingConsignmentResponseData data { get; set; }

        public override string ToString()
        {
            return string.Format("(string) message = {0}\r\n(TrackingConsignmentResponseData) data = \r\n{1}", message.Write(), data.ToString());
        }
    }

    public class CancelConsignmentResponse
    {
        public string message { get; set; }
        public List<object> data { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(string.Format("(string) message = {0}\r\n(List<object>) data (list of {1} object data) =\r\n", message.Write(), (data == null ? "0" : data.Count.ToString())));

            if (data != null)
            {
                for (int i = 1; i <= data.Count; i++)
                {
                    sb.Append(string.Format("Data {0}:\r\n{1}\r\n\r\n", i.ToString(), data[i - 1] != null ? data[i - 1].ToString() : "null"));
                }
            }

            return sb.ToString().TrimEnd();
        }
    }

    public class ServicesResponse
    {
        public string message { get; set; }
        public List<ServicesResponseData> data { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(string.Format("(string) message = {0}\r\ndata (list of {1} ServicesResponseData data) =\r\n", message.Write(), (data == null ? "0" : data.Count.ToString())));

            if (data != null)
            {
                for (int i = 1; i <= data.Count; i++)
                {
                    sb.Append(string.Format("Data {0}:\r\n{1}\r\n\r\n", i.ToString(), data[i - 1].ToString()));
                }
            }

            return sb.ToString().TrimEnd();
        }
    }

    public class TrackingByParcelsResponse
    {
        public string message { get; set; }
        public List<TrackingEvent> data { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(string.Format("(string) message = {0}\r\ndata (list of {1} TrackingEvent data) =\r\n", message.Write(), (data == null ? "0" : data.Count.ToString())));

            if (data != null)
            {
                for (int i = 1; i <= data.Count; i++)
                {
                    sb.Append(string.Format("Data {0}:\r\n{1}\r\n\r\n", i.ToString(), data[i - 1].ToString()));
                }
            }

            return sb.ToString().TrimEnd();
        }
    }

    public class Shipmate
    {
        public Shipmate()
        {
            UserName = ConfigurationManager.AppSettings["ShipmateUserName"];
            Password = ConfigurationManager.AppSettings["ShipmatePassword"];
            Token = ConfigurationManager.AppSettings["ShipmateToken"];
            BaseUrl = ConfigurationManager.AppSettings["ShipmateBaseUrl"];
        }

        public string UserName { get; set; }

        public string Password { get; set; }

        public string Token { get; set; }

        private string BaseUrl { get; set; }

        private T MakeWebRequest<T>(string url, string requestBody)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Accept = "application/json";

            if (string.IsNullOrEmpty(requestBody))
            {
                request.Method = "GET";
            }
            else
            {
                request.Method = "POST";
                byte[] byteArray = Encoding.UTF8.GetBytes(requestBody);
                Stream dataStream = request.GetRequestStream();
                dataStream.Write(byteArray, 0, byteArray.Length);
                dataStream.Close();
            }

            WebResponse response = request.GetResponse();
            Stream responseStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(responseStream);
            string responseFromServer = reader.ReadToEnd();

            if (string.IsNullOrEmpty(responseFromServer))
            {
                throw new Exception("Shipmate returned an empty response");
            }

            T deserializeResult = default(T);

            try
            {
                deserializeResult = (new JavaScriptSerializer()).Deserialize<T>(responseFromServer);
            }
            catch
            {
                throw new Exception(string.Format("Unable to parse the response from Shipmate: {0}", responseFromServer));
            }

            return deserializeResult;
        }

        public string GetUrl(ShipmateAction shipmateAction, string trackingReference = null, string consignmentReference = null)
        {
            switch (shipmateAction)
            {
                case ShipmateAction.Login:
                    return string.Format("{0}login", BaseUrl);
                case ShipmateAction.Services:
                    return string.Format("{0}services?token={1}", BaseUrl, Token);
                case ShipmateAction.CreateConsignment:
                    return string.Format("{0}Consignments", BaseUrl);
                case ShipmateAction.TrackingByConsignment:
                    return string.Format("{0}TrackingByconsignments?Consignments_Reference={1}&Token={2}", BaseUrl, consignmentReference, Token);
                case ShipmateAction.TrackingByParcels:
                    return string.Format("{0}TrackingByTrackingReference?Tracking_Reference={1}&Token={2}", BaseUrl, trackingReference, Token);
                case ShipmateAction.CancelConsignment:
                    return string.Format("{0}cancelconsignments?Reference={1}&Token={2}", BaseUrl, consignmentReference, Token);
                case ShipmateAction.Label:
                    return string.Format("{0}label?Tracking_reference={1}&Token={2}", BaseUrl, trackingReference, Token);
                case ShipmateAction.PrintLabel:
                    return string.Format("{0}PrintLabel?Tracking_reference={1}&Token={2}", BaseUrl, trackingReference, Token);
                default:
                    throw new Exception(string.Format("Unsupported ShipmateAction \"{0}\"", shipmateAction));
            }
        }

        public LoginResponse Login(LoginRequest loginRequest)
        {
            return MakeWebRequest<LoginResponse>(GetUrl(ShipmateAction.Login), loginRequest.ToJson());
        }

        public ServicesResponse GetServices()
        {
            return MakeWebRequest<ServicesResponse>(GetUrl(ShipmateAction.Services), null);
        }

        public ConsignmentResponse CreateConsignment(string saediFromId, ConsignmentRequest consignmentRequest, out int shipmateConsignmentCreationId)
        {
            ConsignmentResponse consignmentResponse;
            OnlineBookingLogBLL onlineBookingLogBLL = new OnlineBookingLogBLL();
            Mobile.Portal.BLL.ShipmateBLL shipmateBLL = new ShipmateBLL();
            shipmateConsignmentCreationId = 0;

            try
            {
                consignmentResponse = MakeWebRequest<ConsignmentResponse>(GetUrl(ShipmateAction.CreateConsignment), consignmentRequest.ToJson());

                if (consignmentResponse.message == "Consignment Created")
                {
                    onlineBookingLogBLL.InsertOnlineServiceLog(consignmentRequest, consignmentResponse, saediFromId, GetUrl(ShipmateAction.CreateConsignment), true);
                    shipmateConsignmentCreationId = shipmateBLL.CreateLogEntry(GetShipmateConsignmentRequestResponse(true, null, consignmentRequest, consignmentResponse), true);
                }
                else
                {
                    onlineBookingLogBLL.InsertOnlineServiceLog(consignmentRequest, consignmentResponse.message + " " + StringHelper.Dump(consignmentResponse), saediFromId, GetUrl(ShipmateAction.CreateConsignment), false);
                    shipmateConsignmentCreationId = shipmateBLL.CreateLogEntry(GetShipmateConsignmentRequestResponse(false, consignmentResponse.message, consignmentRequest, consignmentResponse), true);
                    throw new Exception(consignmentResponse.message);
                }
            }
            catch (Exception e)
            {
                onlineBookingLogBLL.InsertOnlineServiceLog(consignmentRequest, e.Message, saediFromId, GetUrl(ShipmateAction.CreateConsignment), false);
                shipmateBLL.CreateLogEntry(GetShipmateConsignmentRequestResponse(false, e.Message, consignmentRequest, null), false);
                throw;
            }

            return consignmentResponse;
        }

        private ShipmateConsignmentRequestResponse GetShipmateConsignmentRequestResponse(bool success, string errorMessage, ConsignmentRequest consignmentRequest, ConsignmentResponse consignmentResponse)
        {
            ShipmateConsignmentRequestResponse s = new ShipmateConsignmentRequestResponse();

            s.Success = success;
            s.ErrorMessage = errorMessage;
            s.ReqServiceID = consignmentRequest.ServiceID;
            s.ReqRemittanceID = consignmentRequest.RemittanceID;
            s.ReqConsignmentReference = consignmentRequest.consignment_reference;
            s.ReqServiceKey = consignmentRequest.service_key;
            s.ReqName = consignmentRequest.to_address.name;
            s.ReqLine1 = consignmentRequest.to_address.line_1;
            s.ReqCity = consignmentRequest.to_address.city;
            s.ReqPostcode = consignmentRequest.to_address.postcode;
            s.ReqCountry = consignmentRequest.to_address.country;
            s.ReqReference = consignmentRequest.parcels[0].reference;
            s.ReqWeight = consignmentRequest.parcels[0].weight;
            s.ReqWidth = consignmentRequest.parcels[0].width;
            s.ReqLength = consignmentRequest.parcels[0].length;
            s.ReqDepth = consignmentRequest.parcels[0].depth;

            if (consignmentResponse != null)
            {
                s.RepMessage = consignmentResponse.message;
                s.RepConsignmentReference = consignmentResponse.data[0].consignment_reference;
                s.RepParcelReference = consignmentResponse.data[0].parcel_reference;
                s.RepCarrier = consignmentResponse.data[0].carrier;
                s.RepServiceName = consignmentResponse.data[0].service_name;
                s.RepTrackingReference = consignmentResponse.data[0].tracking_reference;
                s.RepCreatedBy = consignmentResponse.data[0].created_by;
                s.RepCreatedWith = consignmentResponse.data[0].created_with;
                s.RepCreatedAt = consignmentResponse.data[0].created_at;
                s.RepDeliveryName = consignmentResponse.data[0].to_address.delivery_name;
                s.RepLine1 = consignmentResponse.data[0].to_address.line_1;
                s.RepLine2 = consignmentResponse.data[0].to_address.line_2;
                s.RepLine3 = consignmentResponse.data[0].to_address.line_3;
                s.RepCity = consignmentResponse.data[0].to_address.city;
                s.RepCounty = consignmentResponse.data[0].to_address.county;
                s.RepPostcode = consignmentResponse.data[0].to_address.postcode;
                s.RepCountry = consignmentResponse.data[0].to_address.country;
                s.RepPdf = consignmentResponse.data[0].pdf;
                s.RepZpl = consignmentResponse.data[0].zpl;
                s.RepPng = consignmentResponse.data[0].png;
            }

            return s;
        }

        public TrackingConsignmentResponse TrackingByConsignment(string consignmentReference)
        {
            return MakeWebRequest<TrackingConsignmentResponse>(GetUrl(ShipmateAction.TrackingByConsignment, consignmentReference: consignmentReference), null);
        }

        public TrackingByParcelsResponse TrackingByParcels(string trackingReference)
        {
            return MakeWebRequest<TrackingByParcelsResponse>(GetUrl(ShipmateAction.TrackingByParcels, trackingReference: trackingReference), null);
        }

        public CancelConsignmentResponse CancelConsignments(string consignmentReference)
        {
            return MakeWebRequest<CancelConsignmentResponse>(GetUrl(ShipmateAction.CancelConsignment, consignmentReference: consignmentReference), null);
        }

        public ConsignmentResponse GetLabel(string trackingReference)
        {
            return MakeWebRequest<ConsignmentResponse>(GetUrl(ShipmateAction.Label, trackingReference: trackingReference), null);
        }

        public ConsignmentResponse PrintLabel(string trackingReference)
        {
            return MakeWebRequest<ConsignmentResponse>(GetUrl(ShipmateAction.PrintLabel, trackingReference: trackingReference), null);
        }
    }
}

namespace Mobile.Portal.BLL
{
    public class ShipmateBLL : BaseBLL<Mobile.Portal.DAL.ShipmateConsignmentRequestResponse>, Mobile.Portal.DAL.IShipmateDataProvider
    {
        Mobile.Portal.DAL.IShipmateDataProvider _dal;

        public ShipmateBLL()
        {
            _dal = new Mobile.Portal.DAL.ShipmateDataProvider();
        }

        public int CreateLogEntry(Mobile.Portal.DAL.ShipmateConsignmentRequestResponse s, bool addResponseParameters)
        {
            return _dal.CreateLogEntry(s, addResponseParameters);
        }
    }
}