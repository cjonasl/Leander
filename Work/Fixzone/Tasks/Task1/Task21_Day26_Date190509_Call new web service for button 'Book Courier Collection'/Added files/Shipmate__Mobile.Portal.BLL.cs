using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Net;
using System.Web.Script.Serialization;
using Mobile.Portal.Helpers;
using Mobile.Portal.DAL;
using System.Security.Cryptography;
using Mobile.Portal.Classes;

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
                return str.Trim();
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

        public static string FormatDate(this DateTime? date)
        {
            if (!date.HasValue)
                return "";
            else
                return date.Value.ToString("dd/MM/yyyy HH:mm:ss");
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

    public class Address
    {
        public string name { get; set; }
        public string line_1 { get; set; }
        public string line_2 { get; set; }
        public string line_3 { get; set; }
        public string company_name { get; set; }
        public string telephone { get; set; }
        public string email_address { get; set; }
        public string city { get; set; }
        public string postcode { get; set; }
        public string country { get; set; }

        public Address(string name, string line1, string line2, string line3, string companyName, string telephone, string emailAddress, string city, string postcode, string country)
        {
            this.name = name.ReplaceNullAndWhiteSpaceWithEmptyString();
            this.line_1 = line1.ReplaceNullAndWhiteSpaceWithEmptyString();
            this.line_2 = line2.ReplaceNullAndWhiteSpaceWithEmptyString();
            this.line_3 = line3.ReplaceNullAndWhiteSpaceWithEmptyString();
            this.company_name = companyName.ReplaceNullAndWhiteSpaceWithEmptyString();
            this.telephone = telephone.ReplaceNullAndWhiteSpaceWithEmptyString();
            this.email_address = emailAddress.ReplaceNullAndWhiteSpaceWithEmptyString();
            this.city = city.ReplaceNullAndWhiteSpaceWithEmptyString();
            this.postcode = postcode.ReplaceNullAndWhiteSpaceWithEmptyString();
            this.country = country.ReplaceNullAndWhiteSpaceWithEmptyString();
        }

        public override string ToString()
        {
            return string.Format("(string) name = {0}\r\n(string) line_1 = {1}\r\n(string) line_2 = {2}\r\n(string) line_3 = {3}\r\n(string) company_name = {4}\r\n(string) telephone = {5}\r\n(string) email_address = {6}\r\n(string) city = {7}\r\n(string) postcode = {8}\r\n(string) country = {9}",
                name.Write(), line_1.Write(), line_2.Write(), line_3.Write(), company_name.Write(), telephone.Write(), email_address.Write(), city.Write(), postcode.Write(), country.Write());
        }

        public string ToJson()
        {
            return (new JavaScriptSerializer()).Serialize(this);
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

    public class CreateConsignmentRequest
    {
        public int ServiceID { get; set; }
        public int RemittanceID { get; set; }
        public string consignment_reference { get; set; }
        public string Token { get; set; }
        public string service_key { get; set; }
        public Address collection_address { get; set; }
        public Address to_address { get; set; }
        public List<Parcel> parcels { get; set; }

        public CreateConsignmentRequest(int serviceID, string consignmentReference, string token, string serviceKey, Address collectionAddress, Address toAddress, List<Parcel> parcels)
        {
            this.ServiceID = serviceID;
            this.RemittanceID = 0;
            this.consignment_reference = consignmentReference.ReplaceNullAndWhiteSpaceWithEmptyString();
            this.Token = token.ReplaceNullAndWhiteSpaceWithEmptyString();
            this.service_key = serviceKey.ReplaceNullAndWhiteSpaceWithEmptyString();
            this.collection_address = collectionAddress;
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
        public string MediaURL { get; set; }
        public string MediaGUID { get; set; }

        public override string ToString()
        {
            return string.Format("(string) consignment_reference = {0}\r\n(string) parcel_reference = {1}\r\n(string) carrier = {2}\r\n(string) service_name = {3}\r\n(string) tracking_reference = {4}\r\n (string) created_by = {5}\r\n(string) created_with = {6} \r\n(DateTime) created_at = {7}\r\n(string) MediaURL = {8}\r\n(string) MediaGUID = {9}",
                consignment_reference.Write(), parcel_reference.Write(), carrier.Write(), service_name.Write(), tracking_reference.Write(), created_by.Write(), created_with.Write(), created_at.ToString(), MediaURL.Write(), MediaGUID.Write());
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

    public class CreateConsignmentResponse
    {
        public string message { get; set; }
        public List<CreateConsignmentResponseData> data { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(string.Format("(string) message = {0}\r\ndata (list of {1} CreateConsignmentResponseData data) =\r\n", message.Write(), (data == null ? "0" : data.Count.ToString())));

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

    public class ShipmateConfig
    {
        public string ShipmateUsername { get; set; }
        public string ShipmatePassword { get; set; }
        public string ShipmateToken { get; set; }
        public string ShipmateServiceKey { get; set; }
        public string ShipmateBaseUrl { get; set; }
        public string CarrierTrackAndTraceUrl { get; set; }
        public string DeliveryToName { get; set; }
        public string DeliveryToLine1 { get; set; }
        public string DeliveryToLine2 { get; set; }
        public string DeliveryToLine3 { get; set; }
        public string DeliveryToCompanyName { get; set; }
        public string DeliveryToTelephone { get; set; }
        public string DeliveryToEmail { get; set; }
        public string DeliveryToCity { get; set; }
        public string DeliveryToPostcode { get; set; }
        public string DeliveryToCountry { get; set; }
    }

    public class ShipmateConsignmentRequestRepsonseDetails
    {
        public string ConsignmentReference { get; set; }
        public string ParcelReference { get; set; }
        public string ServiceID { get; set; }
        public string ServiceKey { get; set; }
        public string TrackingReference { get; set; }
        public string LabelCreated { get; set; }
        public string Manifested { get; set; }
        public string Collected { get; set; }
        public string InTransit { get; set; }
        public string Delivered { get; set; }
        public string DeliveryFailed { get; set; }
        public string Carrier { get; set; }
        public string ServiceName { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedWith { get; set; }
        public string CreatedAt { get; set; }
        public string ParcelWeight { get; set; }
        public string ParcelWidth { get; set; }
        public string ParcelLength { get; set; }
        public string ParcelDepth { get; set; }
        public string MediaURL { get; set; }
        public string MediaGUID { get; set; }
    }

    public class Shipmate
    {
        private ShipmateConfig _shipmateConfig;
        private string _clientId;

        private readonly byte[] encryptorKey = { 239, 172, 233, 89, 121, 55, 70, 175, 83, 250, 36, 213, 16, 139, 196, 146, 117, 221, 136, 132, 91, 222, 69, 101, 5, 72, 64, 93, 234, 209, 30, 122 };
        private readonly byte[] encryptorIV = { 68, 50, 142, 152, 81, 76, 162, 227, 9, 129, 248, 95, 63, 220, 119, 120 };

        public Shipmate() { }

        public Shipmate(string clientId)
        {
            string message;

            _clientId = clientId;

            _shipmateConfig = GetConfig(clientId, out message);

            if (message != "Success")
                throw new Exception(message);
        }

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

        private string GetUrl(ShipmateAction shipmateAction, string trackingReference = null, string consignmentReference = null)
        {
            switch (shipmateAction)
            {
                case ShipmateAction.Login:
                    return string.Format("{0}login", _shipmateConfig.ShipmateBaseUrl);
                case ShipmateAction.Services:
                    return string.Format("{0}services?token={1}", _shipmateConfig.ShipmateBaseUrl, _shipmateConfig.ShipmateToken);
                case ShipmateAction.CreateConsignment:
                    return string.Format("{0}Consignments", _shipmateConfig.ShipmateBaseUrl);
                case ShipmateAction.TrackingByConsignment:
                    return string.Format("{0}TrackingByconsignments?Consignments_Reference={1}&Token={2}", _shipmateConfig.ShipmateBaseUrl, consignmentReference, _shipmateConfig.ShipmateToken);
                case ShipmateAction.TrackingByParcels:
                    return string.Format("{0}TrackingByTrackingReference?Tracking_Reference={1}&Token={2}", _shipmateConfig.ShipmateBaseUrl, trackingReference, _shipmateConfig.ShipmateToken);
                case ShipmateAction.CancelConsignment:
                    return string.Format("{0}cancelconsignments?Reference={1}&Token={2}", _shipmateConfig.ShipmateBaseUrl, consignmentReference, _shipmateConfig.ShipmateToken);
                case ShipmateAction.Label:
                    return string.Format("{0}label?Tracking_reference={1}&Token={2}", _shipmateConfig.ShipmateBaseUrl, trackingReference, _shipmateConfig.ShipmateToken);
                case ShipmateAction.PrintLabel:
                    return string.Format("{0}PrintLabel?Tracking_reference={1}&Token={2}", _shipmateConfig.ShipmateBaseUrl, trackingReference, _shipmateConfig.ShipmateToken);
                default:
                    throw new Exception(string.Format("Unsupported ShipmateAction \"{0}\"", shipmateAction));
            }
        }

        private string Encrypt(string strDecrypted)
        {
            string strEncrypted;
            byte[] bytes = Encoding.Unicode.GetBytes(strDecrypted);
            using (Aes encryptor = Aes.Create())
            {
                encryptor.Key = encryptorKey;
                encryptor.IV = encryptorIV;
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(bytes, 0, bytes.Length);
                        cs.Close();
                    }
                    strEncrypted = Convert.ToBase64String(ms.ToArray());
                }
            }
            return strEncrypted;
        }

        private string Decrypt(string strEncrypted)
        {
            string strDecrypted;
            byte[] bytes = Convert.FromBase64String(strEncrypted.Replace(" ", "+"));
            using (Aes encryptor = Aes.Create())
            {
                encryptor.Key = encryptorKey;
                encryptor.IV = encryptorIV;
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(bytes, 0, bytes.Length);
                        cs.Close();
                    }
                    strDecrypted = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return strDecrypted;
        }

        public string SetConfig(string clientId, string shipmateUsername, string shipmatePassword, string shipmateToken, string shipmateServiceKey, string shipmateBaseUrl, string carrierTrackAndTraceUrl, string DeliveryToName, string DeliveryToLine1, string DeliveryToLine2, string DeliveryToLine3, string DeliveryToCompanyName, string DeliveryToTelephone, string DeliveryToEmail, string DeliveryToCity, string DeliveryToPostcode, string DeliveryToCountry)
        {
            ShipmateConfig config = new ShipmateConfig()
            {
                ShipmateUsername = shipmateUsername,
                ShipmatePassword = Encrypt(shipmatePassword),
                ShipmateToken = Encrypt(shipmateToken),
                ShipmateServiceKey = shipmateServiceKey,
                ShipmateBaseUrl = shipmateBaseUrl,
                CarrierTrackAndTraceUrl = carrierTrackAndTraceUrl,
                DeliveryToName = DeliveryToName,
                DeliveryToLine1 = DeliveryToLine1,
                DeliveryToLine2 = DeliveryToLine2,
                DeliveryToLine3 = DeliveryToLine3,
                DeliveryToCompanyName = DeliveryToCompanyName,
                DeliveryToTelephone = DeliveryToTelephone,
                DeliveryToEmail = DeliveryToEmail,
                DeliveryToCity = DeliveryToCity,
                DeliveryToPostcode = DeliveryToPostcode,
                DeliveryToCountry = DeliveryToCountry
            };

            ShipmateConfigBLL shipmateConfigBLL = new ShipmateConfigBLL();
            return shipmateConfigBLL.ShipmateConfig(clientId: clientId, config: (new JavaScriptSerializer()).Serialize(config));
        }

        public ShipmateConfig GetConfig(string clientId, out string message)
        {
            ShipmateConfig shipmateConfig = null;
            ShipmateConfigBLL shipmateConfigBLL = new ShipmateConfigBLL();
            string result = shipmateConfigBLL.ShipmateConfig(clientId: clientId);

            if (result == "ClientId does not exist!")
            {
                message = "ClientId does not exist!";
            }
            else if (string.IsNullOrEmpty(result))
            {
                message = "Config is empty";
            }
            else
            {
                message = "Success";
                shipmateConfig = (new JavaScriptSerializer()).Deserialize<ShipmateConfig>(result);
                shipmateConfig.ShipmatePassword = Decrypt(shipmateConfig.ShipmatePassword);
                shipmateConfig.ShipmateToken = Decrypt(shipmateConfig.ShipmateToken);
            }

            return shipmateConfig;
        }

        public LoginResponse Login(LoginRequest loginRequest)
        {
            return MakeWebRequest<LoginResponse>(GetUrl(ShipmateAction.Login), loginRequest.ToJson());
        }

        public ServicesResponse GetServices()
        {
            return MakeWebRequest<ServicesResponse>(GetUrl(ShipmateAction.Services), null);
        }

        public string CreateConsignment(string saediFromId, CreateConsignmentRequest createConsignmentRequest, out int shipmateConsignmentCreationId)
        {
            CreateConsignmentResponse createConsignmentResponse;
            OnlineBookingLogBLL onlineBookingLogBLL = new OnlineBookingLogBLL();
            ShipmateConsignmentDetailsBLL shipmateBLL = new ShipmateConsignmentDetailsBLL();
            shipmateConsignmentCreationId = 0;
            string trackingReference = null;

            try
            {
                createConsignmentRequest.Token = _shipmateConfig.ShipmateToken;
                createConsignmentResponse = MakeWebRequest<CreateConsignmentResponse>(GetUrl(ShipmateAction.CreateConsignment), createConsignmentRequest.ToJson());

                if (createConsignmentResponse.message == "Consignment Created")
                {
                    onlineBookingLogBLL.InsertOnlineServiceLog(createConsignmentRequest, createConsignmentResponse, saediFromId, GetUrl(ShipmateAction.CreateConsignment), true);
                    DateTime? labelCreated = GetLabelCreated(createConsignmentResponse.data[0].tracking_reference);
                    trackingReference = createConsignmentResponse.data[0].tracking_reference;
                    shipmateConsignmentCreationId = shipmateBLL.CreateLogEntry(GetShipmateConsignmentDetails(true, null, createConsignmentRequest, createConsignmentResponse, labelCreated), true);
                }
                else
                {
                    onlineBookingLogBLL.InsertOnlineServiceLog(createConsignmentRequest, createConsignmentResponse.message + " " + StringHelper.Dump(createConsignmentResponse), saediFromId, GetUrl(ShipmateAction.CreateConsignment), false);
                    shipmateConsignmentCreationId = shipmateBLL.CreateLogEntry(GetShipmateConsignmentDetails(false, createConsignmentResponse.message, createConsignmentRequest, null, null), false);
                }
            }
            catch (Exception e)
            {
                onlineBookingLogBLL.InsertOnlineServiceLog(createConsignmentRequest, e.Message, saediFromId, GetUrl(ShipmateAction.CreateConsignment), false);
                shipmateBLL.CreateLogEntry(GetShipmateConsignmentDetails(false, e.Message, createConsignmentRequest, null, null), false);
                throw;
            }

            if (createConsignmentResponse.message != "Consignment Created")
                throw new Exception(createConsignmentResponse.message);

            return trackingReference;
        }

        public CreateConsignmentRequest GetCreateConsignmentRequest(string saediFromId, string rmaId, string clientRef, out string onlineBookingURL)
        {
            ClientBLL clientBLL = new ClientBLL();
            Client client = clientBLL.GetBySaediId(saediFromId, programVersion: "2");
            Address collectionAddress, toAddress;

            onlineBookingURL = client.OspRef.OnlineBookingURL;

            string line1 = client.DeliveryAddress.Address1.ReplaceNullAndWhiteSpaceWithEmptyString();
            string line2 = client.DeliveryAddress.Address2.ReplaceNullAndWhiteSpaceWithEmptyString();
            string line3 = "";

            string[] address = new string[4];
            address[0] = client.DeliveryAddress.Address3.ReplaceNullAndWhiteSpaceWithEmptyString();
            address[1] = client.DeliveryAddress.Address4.ReplaceNullAndWhiteSpaceWithEmptyString();
            address[2] = client.DeliveryAddress.Address5.ReplaceNullAndWhiteSpaceWithEmptyString();
            address[3] = client.DeliveryAddress.Additional.ReplaceNullAndWhiteSpaceWithEmptyString();

            for (int i = 0; i < 4; i++)
            {
                if (line3 != "" && address[i] != "")
                    line3 += (" " + address[i]);
                else if (address[i] != "")
                    line3 = address[i];
            }

            collectionAddress = new Address(client.CompanyName, line1, line2, line3, "", client.TelNo, client.EmailAddress, client.DeliveryAddress.City, client.DeliveryAddress.PostalCode, client.DeliveryAddress.Country);

            toAddress = new Address(_shipmateConfig.DeliveryToName,
                                    _shipmateConfig.DeliveryToLine1,
                                    _shipmateConfig.DeliveryToLine2,
                                    _shipmateConfig.DeliveryToLine3,
                                    _shipmateConfig.DeliveryToCompanyName,
                                    _shipmateConfig.DeliveryToTelephone,
                                    _shipmateConfig.DeliveryToEmail,
                                    _shipmateConfig.DeliveryToCity,
                                    _shipmateConfig.DeliveryToPostcode,
                                    _shipmateConfig.DeliveryToCountry);

            return new CreateConsignmentRequest(int.Parse(clientRef), rmaId, null, _shipmateConfig.ShipmateServiceKey, collectionAddress, toAddress, null);
        }

        private ShipmateConsignmentDetails GetShipmateConsignmentDetails(bool sendRequestSuccess, string errorMessage, CreateConsignmentRequest createConsignmentRequest, CreateConsignmentResponse createConsignmentResponse, DateTime? labelCreated)
        {
            ShipmateConsignmentDetails scd = new ShipmateConsignmentDetails();

            scd.ClientId = _clientId;
            scd.SendRequestSuccess = sendRequestSuccess;
            scd.SendRequestErrorMessage = errorMessage;

            if (sendRequestSuccess)
                scd.ResTrackingReference = createConsignmentResponse.data[0].tracking_reference;

            scd.LabelCreated = labelCreated;
            scd.ReqServiceID = createConsignmentRequest.ServiceID;
            scd.ReqRemittanceID = createConsignmentRequest.RemittanceID;
            scd.ReqConsignmentReference = createConsignmentRequest.consignment_reference;
            scd.ReqServiceKey = createConsignmentRequest.service_key;
            scd.ReqCollectionFromName = createConsignmentRequest.collection_address.name;
            scd.ReqCollectionFromLine1 = createConsignmentRequest.collection_address.line_1;
            scd.ReqCollectionFromLine2 = createConsignmentRequest.collection_address.line_2;
            scd.ReqCollectionFromLine3 = createConsignmentRequest.collection_address.line_3;
            scd.ReqCollectionFromCompanyName = createConsignmentRequest.collection_address.company_name;
            scd.ReqCollectionFromTelephone = createConsignmentRequest.collection_address.telephone;
            scd.ReqCollectionFromEmailAddress = createConsignmentRequest.collection_address.email_address;
            scd.ReqCollectionFromCity = createConsignmentRequest.collection_address.city;
            scd.ReqCollectionFromPostcode = createConsignmentRequest.collection_address.postcode;
            scd.ReqCollectionFromCountry = createConsignmentRequest.collection_address.country;
            scd.ReqDeliveryToName = createConsignmentRequest.to_address.name;
            scd.ReqDeliveryToLine1 = createConsignmentRequest.to_address.line_1;
            scd.ReqDeliveryToLine2 = createConsignmentRequest.to_address.line_2;
            scd.ReqDeliveryToLine3 = createConsignmentRequest.to_address.line_3;
            scd.ReqDeliveryToCompanyName = createConsignmentRequest.to_address.company_name;
            scd.ReqDeliveryToTelephone = createConsignmentRequest.to_address.telephone;
            scd.ReqDeliveryToEmailAddress = createConsignmentRequest.to_address.email_address;
            scd.ReqDeliveryToCity = createConsignmentRequest.to_address.city;
            scd.ReqDeliveryToPostcode = createConsignmentRequest.to_address.postcode;
            scd.ReqDeliveryToCountry = createConsignmentRequest.to_address.country;
            scd.ReqParcelReference = createConsignmentRequest.parcels[0].reference;
            scd.ReqParcelWeight = createConsignmentRequest.parcels[0].weight;
            scd.ReqParcelWidth = createConsignmentRequest.parcels[0].width;
            scd.ReqParcelLength = createConsignmentRequest.parcels[0].length;
            scd.ReqParcelDepth = createConsignmentRequest.parcels[0].depth;

            if (sendRequestSuccess)
            {
                scd.ResMessage = createConsignmentResponse.message;
                scd.ResConsignmentReference = createConsignmentResponse.data[0].consignment_reference;
                scd.ResParcelReference = createConsignmentResponse.data[0].parcel_reference;
                scd.ResCarrier = createConsignmentResponse.data[0].carrier;
                scd.ResServiceName = createConsignmentResponse.data[0].service_name;
                scd.ResCreatedBy = createConsignmentResponse.data[0].created_by;
                scd.ResCreatedWith = createConsignmentResponse.data[0].created_with;
                scd.ResCreatedAt = createConsignmentResponse.data[0].created_at;
                scd.ResMediaURL = createConsignmentResponse.data[0].MediaURL;
                scd.ResMediaGUID = createConsignmentResponse.data[0].MediaGUID;
            }

            return scd;
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

        public CreateConsignmentResponse GetLabel(string trackingReference)
        {
            return MakeWebRequest<CreateConsignmentResponse>(GetUrl(ShipmateAction.Label, trackingReference: trackingReference), null);
        }

        public CreateConsignmentResponse PrintLabel(string trackingReference)
        {
            return MakeWebRequest<CreateConsignmentResponse>(GetUrl(ShipmateAction.PrintLabel, trackingReference: trackingReference), null);
        }

        public object GetShipmateConsignmentDetails(string trackingReference)
        {
            return (new ShipmateConsignmentDetailsBLL()).GetShipmateConsignmentDetails(trackingReference);
        }

        private DateTime? GetLabelCreated(string trackingReference)
        {
            TrackingByParcelsResponse trackingByParcelsResponse = TrackingByParcels(trackingReference);
            TrackingEvent trackingEvent = trackingByParcelsResponse.data.FirstOrDefault(x => x.type == "LABEL_CREATED");

            return trackingEvent != null ? (DateTime?)trackingEvent.date : null;
        }

        public ShipmateConsignmentRequestRepsonseDetails GetShipmateConsignmentRequestRepsonseDetails(object scd)
        {
            return new ShipmateConsignmentRequestRepsonseDetails()
            {
                ConsignmentReference = ((ShipmateConsignmentDetails)scd).ReqConsignmentReference,
                ParcelReference = ((ShipmateConsignmentDetails)scd).ResParcelReference,
                ServiceID = ((ShipmateConsignmentDetails)scd).ReqServiceID.ToString(),
                ServiceKey = ((ShipmateConsignmentDetails)scd).ReqServiceKey,
                TrackingReference = ((ShipmateConsignmentDetails)scd).ResTrackingReference,
                LabelCreated = ((ShipmateConsignmentDetails)scd).LabelCreated.FormatDate(),
                Manifested = ((ShipmateConsignmentDetails)scd).Manifested.FormatDate(),
                Collected = ((ShipmateConsignmentDetails)scd).Collected.FormatDate(),
                InTransit = ((ShipmateConsignmentDetails)scd).InTransit.FormatDate(),
                Delivered = ((ShipmateConsignmentDetails)scd).Delivered.FormatDate(),
                DeliveryFailed = ((ShipmateConsignmentDetails)scd).DeliveryFailed.FormatDate(),
                Carrier = ((ShipmateConsignmentDetails)scd).ResCarrier,
                ServiceName = ((ShipmateConsignmentDetails)scd).ResServiceName,
                CreatedBy = ((ShipmateConsignmentDetails)scd).ResCreatedBy,
                CreatedWith = ((ShipmateConsignmentDetails)scd).ResCreatedWith,
                CreatedAt = ((ShipmateConsignmentDetails)scd).ResCreatedAt.FormatDate(),
                ParcelWeight = ((ShipmateConsignmentDetails)scd).ReqParcelWeight.ToString(),
                ParcelWidth = ((ShipmateConsignmentDetails)scd).ReqParcelWidth.ToString(),
                ParcelLength = ((ShipmateConsignmentDetails)scd).ReqParcelLength.ToString(),
                ParcelDepth = ((ShipmateConsignmentDetails)scd).ReqParcelDepth.ToString(),
                MediaURL = ((ShipmateConsignmentDetails)scd).ResMediaURL,
                MediaGUID = ((ShipmateConsignmentDetails)scd).ResMediaGUID
            };
        }

        public Address GetCollectionFromAddress(object scd)
        {
            return new Address
                (
                  ((ShipmateConsignmentDetails)scd).ReqCollectionFromName,
                  ((ShipmateConsignmentDetails)scd).ReqCollectionFromLine1,
                  ((ShipmateConsignmentDetails)scd).ReqCollectionFromLine2,
                  ((ShipmateConsignmentDetails)scd).ReqCollectionFromLine3,
                  ((ShipmateConsignmentDetails)scd).ReqCollectionFromCompanyName,
                  ((ShipmateConsignmentDetails)scd).ReqCollectionFromTelephone,
                  ((ShipmateConsignmentDetails)scd).ReqCollectionFromEmailAddress,
                  ((ShipmateConsignmentDetails)scd).ReqCollectionFromCity,
                  ((ShipmateConsignmentDetails)scd).ReqCollectionFromPostcode,
                  ((ShipmateConsignmentDetails)scd).ReqCollectionFromCountry

                );
        }

        public Address GetDeliveryToAddress(object scd)
        {
            return new Address
                (
                  ((ShipmateConsignmentDetails)scd).ReqDeliveryToName,
                  ((ShipmateConsignmentDetails)scd).ReqDeliveryToLine1,
                  ((ShipmateConsignmentDetails)scd).ReqDeliveryToLine2,
                  ((ShipmateConsignmentDetails)scd).ReqDeliveryToLine3,
                  ((ShipmateConsignmentDetails)scd).ReqDeliveryToCompanyName,
                  ((ShipmateConsignmentDetails)scd).ReqDeliveryToTelephone,
                  ((ShipmateConsignmentDetails)scd).ReqDeliveryToEmailAddress,
                  ((ShipmateConsignmentDetails)scd).ReqDeliveryToCity,
                  ((ShipmateConsignmentDetails)scd).ReqDeliveryToPostcode,
                  ((ShipmateConsignmentDetails)scd).ReqDeliveryToCountry
                );
        }

        public string GetCarrierTrackAndTraceUrl(string trackingReference)
        {
            ShipmateConfigBLL shipmateConfigBLL = new ShipmateConfigBLL();
            string result = shipmateConfigBLL.ShipmateConfig(trackingReference : trackingReference);
            ShipmateConfig shipmateConfig = (new JavaScriptSerializer()).Deserialize<ShipmateConfig>(result);
            return shipmateConfig.CarrierTrackAndTraceUrl;
        }
    }
}

namespace Mobile.Portal.BLL
{
    public class ShipmateConfigBLL : BaseBLL<string>, Mobile.Portal.DAL.IShipmateConfigProvider
    {
        Mobile.Portal.DAL.IShipmateConfigProvider _dal;

        public ShipmateConfigBLL()
        {
            _dal = new Mobile.Portal.DAL.ShipmateConfigProvider();
        }

        public string ShipmateConfig(string clientId = null, string config = null, string trackingReference = null)
        {
            return _dal.ShipmateConfig(clientId, config, trackingReference);
        }
    }

    public class ShipmateConsignmentDetailsBLL : BaseBLL<Mobile.Portal.DAL.ShipmateConsignmentDetails>, Mobile.Portal.DAL.IShipmateDataProvider
    {
        Mobile.Portal.DAL.IShipmateDataProvider _dal;

        public ShipmateConsignmentDetailsBLL()
        {
            _dal = new Mobile.Portal.DAL.ShipmateDataProvider();
        }

        public int CreateLogEntry(ShipmateConsignmentDetails data, bool addResponseParameters)
        {
            return _dal.CreateLogEntry(data, addResponseParameters);
        }

        public Mobile.Portal.DAL.ShipmateConsignmentDetails GetShipmateConsignmentDetails(string trackingReference)
        {
            return _dal.GetShipmateConsignmentDetails(trackingReference);
        }
    }
}