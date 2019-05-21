using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Text;
using System.Net;
using System.Web.Script.Serialization;

namespace Mobile.Portal.BLL.FzShipMate
{
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

            for(int i = 0; i < list.Count; i++)
            {
                if (i == 0)
                    sb.Append(list[i]);
                else
                    sb.Append(", " + list[i]);
            }

            sb.Append("]");

            return sb.ToString();
        }

        public static void Print(string fileNameFullPath, string fileContents)
        {
            FileStream fileStream = new FileStream(fileNameFullPath, FileMode.Create, FileAccess.Write);
            StreamWriter streamWriter = new StreamWriter(fileStream, Encoding.UTF8);
            streamWriter.Write(fileContents);
            streamWriter.Flush();
            fileStream.Flush();
            streamWriter.Close();
            fileStream.Close();
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
            return string.Format("name = {0}\r\nline_1 = {1}\r\ncity = {2}\r\npostCode = {3}\r\ncountry = {4}", name, line_1, city, postcode, country);
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
            return string.Format("delivery_name = {0}\r\nline_1 = {1}\r\nline_2 = {2}\r\nline_3 = {3}\r\ncity = {4}\r\ncounty = {5}\r\npostCode = {6}\r\ncountry = {7}",
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
            return string.Format("reference = {0}\r\nweight = {1}\r\nwidth = {2}\r\nlength = {3}\r\ndepth = {4}", reference, weight, width, length, depth);
        }

        public string ToJson()
        {
            return (new JavaScriptSerializer()).Serialize(this);
        }
    }

    public class ConsignmentRequestData
    {
        public string consignment_reference { get; set; }
        public string Token { get; set; }
        public string service_key { get; set; }
        public ToAddressRequest to_address { get; set; }
        public List<Parcel> parcels { get; set; }

        public ConsignmentRequestData(string consignmentReference, string token, string serviceKey, ToAddressRequest toAddress, List<Parcel> parcels)
        {
            this.consignment_reference = consignmentReference.ReplaceNullAndWhiteSpaceWithEmptyString();
            this.Token = token.ReplaceNullAndWhiteSpaceWithEmptyString();
            this.service_key = serviceKey.ReplaceNullAndWhiteSpaceWithEmptyString();
            this.to_address = toAddress;
            this.parcels = parcels;
        }

        public override string ToString()
        {
            return string.Format("ConsignmentReference = {0}\r\nToken = {1}\r\nServiceKey = {2}\r\nToAddress =\r\n{3}\r\nParcels =\r\n{4}", 
                consignment_reference, Token, service_key, to_address.ToString(), parcels.ToString());
        }

        public string ToJson()
        {
            return (new JavaScriptSerializer()).Serialize(this);
        }
    }

    public class ServicesResponseData
    {
        public string id {get; set;}
        public string name {get; set;}
        public string description {get; set;}
        public string key {get; set;}
        public string carrier {get; set;}
        public List<string> conditions { get; set; }

        public override string ToString()
        {
            return string.Format("id = {0}\r\nname = {1}\r\ndescription = {2}\r\nkey = {3}\r\ncarrier = {4}\r\nconditions = {5}",
                id.Write(), name.Write(), description.Write(), key.Write(), carrier.Write(), Utility.PrintListOfStrings(conditions));
        }
    }

    public class CreateConsignmentResponseData
    {
        public string consignment_reference {get; set;}
        public string parcel_reference {get; set;}
        public string carrier {get; set;}
        public string service_name {get; set;}
        public string tracking_reference {get; set;}
        public string created_by {get; set;}
        public string created_with {get; set;}
        public DateTime created_at {get; set;}
        public ToAddressResponse to_address { get; set; }
        public string pdf { get; set; }
        public string zpl { get; set; }
        public string png { get; set; }

        public override string ToString()
        {
            return string.Format("consignment_reference = {0}\r\nparcel_reference = {1}\r\ncarrier = {2}\r\nservice_name = {3}\r\ntracking_reference = {4}\r\ncreated_by = {5}\r\ncreated_with = {6} \r\ncreated_at = {7} \r\nto_address =\r\n{8}\r\npdf = {9}\r\nzpl = {10}\r\npng = {11}",
                consignment_reference.Write(), parcel_reference.Write(), carrier.Write(), service_name.Write(), tracking_reference.Write(), created_by.Write(), created_with.Write(), created_at.ToString(), to_address.ToString(), pdf.Write(), zpl.Write(), png.Write());
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
            return string.Format("code = {0}\r\nname = {1}\r\ndescription = {2}\r\ndate = {3}\r\ntype = {4}", code.Write(), name.Write(), description.Write(), date.ToString(), type.Write());
        }
    }

    public class TrackingConsignmentResponseData
    {
        public string sender_reference { get; set; }
        public string courier_tracking_reference { get; set; }
        public List<TrackingEvent> tracking_events { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(string.Format("sender_reference = {0}\r\ncourier_tracking_reference = {1}\r\ntracking_events (list of {2} TrackingEvent data) =\r\n",
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

    public class ConsignmentResponse
    {
        public string message { get; set; }
        public List<CreateConsignmentResponseData> data { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(string.Format("message = {0}\r\ndata (list of {1} ConsignmentResponseData data) =\r\n", message.Write(), (data == null ? "0" : data.Count.ToString())));

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
            return string.Format("message = {0}\r\ndata = \r\n{1}", message.Write(), data.ToString());
        }
    }

    public class CancelConsignmentResponse
    {
        public string message { get; set; }
        public List<object> data { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(string.Format("message = {0}\r\ndata (list of {1} object data) =\r\n", message.Write(), (data == null ? "0" : data.Count.ToString())));

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
            StringBuilder sb = new StringBuilder(string.Format("message = {0}\r\ndata (list of {1} ServicesResponseData data) =\r\n", message.Write(), (data == null ? "0" : data.Count.ToString())));

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

    public class FzShipMate
    {
        private enum FzShipMateAction
        {
            Services,
            CreateConsignment,
            TrackingConsignment,
            CancelConsignment,
            Label
        }

        public FzShipMate()
        {
            Token = ConfigurationManager.AppSettings["FzShipMateToken"];
            BaseUrl = ConfigurationManager.AppSettings["FzShipMateBaseUrl"];
        }

        public string Token { get; set; }

        private string BaseUrl { get; set; }

        private T MakeWebRequest<T>(string url, string requestBody)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
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

            return (new JavaScriptSerializer()).Deserialize<T>(responseFromServer);
        }

        private string GetUrl(FzShipMateAction fzShipMateAction, string trackingReference = null, string consignmentReference = null)
        {
            switch (fzShipMateAction)
            {
                case FzShipMateAction.Services:
                    return string.Format("{0}services?token={1}", BaseUrl, Token);
                case FzShipMateAction.CreateConsignment:
                    return string.Format("{0}Consignments", BaseUrl);
                case FzShipMateAction.TrackingConsignment:
                    return string.Format("{0}TrackingByconsignments?Consignments_Reference={1}&Token={2}", BaseUrl, consignmentReference, Token);
                case FzShipMateAction.CancelConsignment:
                    return string.Format("{0}cancelconsignments?Reference={1}&Token={2}", BaseUrl, consignmentReference, Token);
                case FzShipMateAction.Label:
                    return string.Format("{0}label?Tracking_reference={1}&Token={2}", BaseUrl, trackingReference, Token);
                default:
                    throw new Exception(string.Format("Unsupported FzShipMateAction \"{0}\"", fzShipMateAction));
            }
        }

        public ServicesResponse GetServices()
        {
            return MakeWebRequest<ServicesResponse>(GetUrl(FzShipMateAction.Services), null);
        }

        public ConsignmentResponse CreateConsignment(ConsignmentRequestData consignmentRequestData)
        {
            return MakeWebRequest<ConsignmentResponse>(GetUrl(FzShipMateAction.CreateConsignment), consignmentRequestData.ToJson());
        }

        public CancelConsignmentResponse TrackingConsignment(string consignmentReference)
        {
            return MakeWebRequest<CancelConsignmentResponse>(GetUrl(FzShipMateAction.TrackingConsignment, consignmentReference: consignmentReference), null);
        }

        public CancelConsignmentResponse CancelConsignments(string consignmentReference)
        {
            return MakeWebRequest<CancelConsignmentResponse>(GetUrl(FzShipMateAction.CancelConsignment, consignmentReference: consignmentReference), null);
        }

        public ConsignmentResponse GetLabel(string trackingReference)
        {
            return MakeWebRequest<ConsignmentResponse>(GetUrl(FzShipMateAction.Label, trackingReference: trackingReference), null);
        }
    }
}