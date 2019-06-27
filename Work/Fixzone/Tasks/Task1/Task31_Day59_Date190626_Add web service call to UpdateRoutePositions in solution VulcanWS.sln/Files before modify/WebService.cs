using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vulcan.Classes;
using Vulcan.Exceptions;
using System.IO;
using System.Xml;
using Vulcan.WebService.OnlineBookingService;

namespace Vulcan.WebService
{

    public class OnlineSpareParts
    {
        public const string ospPwd = "fecker";
        private static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        //Random rnd = new Random();             // used for dev/test only               

        public OnlineOrder OrderPartsByServiceToEng(string userId, string supplierId, string clientRef, OnlineOrder onlineOrder, string url)
        {
            onlineOrder.address.addressType = 1;
            return OrderPartsByService(userId, supplierId, clientRef, onlineOrder, url);
        }


        public OnlineOrder OrderPartsByService(string userId, string supplierId, string clientRef, OnlineOrder onlineOrder, string url)
        {
            int partLineNo = 0;
            int vanLineNo = 0;

            OnlinePartsService.IOnlineSparePartsservice OSP = new OnlinePartsService.IOnlineSparePartsservice();

            if (OSP == null)
            {
                return onlineOrder;
            }
            OSP.Url = url;


            string[] orderLines = new string[0];
            string[] vanLines = new string[0];

            foreach (OnlineOrderPart orderLine in onlineOrder.parts)
            {
                string partNote = string.Empty;
                try
                {
                    partNote = orderLine.partNote.Replace(",", string.Empty);
                }
                catch { }

                string str = //"'" + partLineNo + "'" + "," +     // 0 ... Line No
                    "'" + orderLine.partRef + "'" + "," +         // 1 ... PartRef / StockId
                    "'" + orderLine.quantity + "'" + "," +        // 2 ... Quantity 
                    "'" + onlineOrder.customerFit + "'" + "," +   // 3 ... Customer to fit
                    "'" + orderLine.partDateTime + "'" + "," +    // 4 ... Engineer Part Id
                    "'" + string.Empty + "'" + "," +              // 5 ... Client SAEDI Id
                    "'" + partNote + "'" + "," +                  // 6 ... 
                    "'" + orderLine.orderReason + "'";            // 7 ... Order Reason


                if (!String.IsNullOrWhiteSpace(orderLine.transactionNo) && orderLine.transactionNo.StartsWith("VAN#", StringComparison.OrdinalIgnoreCase))
                {
                    vanLineNo++;
                    Array.Resize(ref vanLines, vanLines.Length + 1);
                    vanLines[vanLines.Length - 1] = "'" + vanLineNo + "'," + str;
                }
                else if (orderLine.partRef != 0)
                {
                    partLineNo++;
                    Array.Resize(ref orderLines, orderLines.Length + 1);
                    orderLines[orderLines.Length - 1] = "'" + partLineNo + "'," + str;
                }
            }

            string addressTypeAsString;
            if (onlineOrder.address.addressType <= 1)
            {
                addressTypeAsString = "Engineer";
            }
            else if (onlineOrder.address.addressType == 2)
            {
                addressTypeAsString = "Customer";
            }
            else
            {
                addressTypeAsString = String.Empty;
            }

            if (vanLineNo > 0)
            {
                string records = OSP.OrderMultipleVanParts(userId.ToUpper(), ospPwd, "", vanLines, Convert.ToInt32(clientRef));

                byte[] byteArray = Encoding.ASCII.GetBytes(records);
                MemoryStream stream = new MemoryStream(byteArray);

                XmlTextReader xml = new XmlTextReader(stream);
                while (xml.Read())
                {
                    onlineOrder.status = 2;
                    onlineOrder.result = "Sent";
                }
            }


            if (partLineNo > 0)
            {
                string records = OSP.OrderMultipleSpareParts(
                    userId.ToUpper(), ospPwd, "",
                    orderLines,
                    Convert.ToInt32(clientRef),
                    "",
                    onlineOrder.address.name,
                    onlineOrder.address.addressLine1,
                    onlineOrder.address.addressLine2,
                    onlineOrder.address.addressLine3,
                    onlineOrder.address.addressLine4,
                    onlineOrder.address.postalCode,
                    onlineOrder.address.additionalNote,
                    addressTypeAsString);

                byte[] byteArray = Encoding.ASCII.GetBytes(records);
                MemoryStream stream = new MemoryStream(byteArray);



                XmlTextReader xml = new XmlTextReader(stream);
                while (xml.Read())
                {
                    if ((xml.Name == "OrderList") && (xml.NodeType == XmlNodeType.Element))
                    {
                        if (xml.GetAttribute("OrderID").ToString() == "~None~" || xml.GetAttribute("OrderID").ToString() == "SOR000000")
                        {
                            onlineOrder.status = 2;
                            onlineOrder.result = "Sent";
                        }
                        else
                        {
                            onlineOrder.status = 1;
                            onlineOrder.result = "Error";
                        }
                    }

                    if ((xml.Name == "Order") && (xml.NodeType == XmlNodeType.Element))
                    {
                        OnlineOrderPart line = onlineOrder.parts.Find(delegate(OnlineOrderPart cp) { return cp.partRef == int.Parse(xml.GetAttribute("StockCode")); });
                        if (line != null)
                        {
                            line.transactionNo = xml.GetAttribute("ErrDescription");
                        }
                    }
                }
            }
            return onlineOrder;
        }

        public List<Part> FindPartsByService(string fromId, string clientRef, string searchText, string url,int Pagesize)
        {
            List<Part> list = new List<Part>();
                if (!String.IsNullOrWhiteSpace(url))
                {
                    OnlinePartsService.IOnlineSparePartsservice OSP = new OnlinePartsService.IOnlineSparePartsservice();
                    if (OSP == null)
                    {
                        return list;
                    }

                    OSP.Url = url;

                    // Restricted to first 250 records
                    string upperFromId = fromId.ToUpper();
                    string searchPattern = searchText + "%";

                    string records = OSP.QuerySpareParts(upperFromId, ospPwd, clientRef, "B", searchPattern, Pagesize, 1, "");

                    // --- the Delphi OSP Webservice sometimes returns ill formed SOAP packet
                    if (String.IsNullOrWhiteSpace(records))
                    {
                        throw new VulcanException("The specified OSP webservice did not return a resultset");
                    }
                    // --- the Delphi OSP WebService sometimes corrupts the XMLString when only one record is found, this fixes that up 
                    records = records.Replace("&lt;![CDATA[", "<![CDATA[");
                    records = records.Replace("&lt;PartsList>", "<PartsList>");
                    records = records.Replace("&lt;/PartsList>", "</PartsList>");
                    records = records.Replace("&lt;Part StkCode", "<Part StkCode");
                    records = records.Replace("&lt;/Part>", "</Part>");
                    // ---

                    byte[] byteArray = Encoding.ASCII.GetBytes(records);
                    MemoryStream stream = new MemoryStream(byteArray);

                    XmlTextReader xml = new XmlTextReader(stream);

                    while (xml.Read())
                    {
                        if (xml.AttributeCount != 0)
                        {
                            Part item = new Part();
                            try { item.lineNumber = list.Count() + 1; }
                            catch
                            {  // why is nothing here!??! (TODO) 
                            }
                            try { item.stockCode = xml.GetAttribute("StkCode").Trim(); }
                            catch
                            {  // why is nothing here!??! (TODO)  
                            }
                            try { item.description = xml.GetAttribute("StkDescription").Trim(); }
                            catch
                            {  // why is nothing here!??! (TODO) 
                            }
                            try { item.partRef = int.Parse(xml.GetAttribute("StockID")); }
                            catch
                            {  // why is nothing here!??! (TODO)  
                            }
                            try { item.saleValue = decimal.Parse(xml.GetAttribute("StkSalePrice")); }
                            catch
                            {  // why is nothing here!??! (TODO) 
                            }
                            try { item.costValue = decimal.Parse(xml.GetAttribute("StkCostValue")); }
                            catch
                            {  // why is nothing here!??! (TODO) 
                            }
                            try { item.quantity = int.Parse(xml.GetAttribute("StkQuantInStock")); }
                            catch
                            {  // why is nothing here!??! (TODO)  
                            }
                            try { item.vanQuantity = int.Parse(xml.GetAttribute("VanQuantInStock")); }
                            catch
                            {  // why is nothing here!??! (TODO) 
                            }
                            try { item.stkQtyInStock = int.Parse(xml.GetAttribute("StkQuantInStock")); }
                            catch
                            {  // why is nothing here!??! (TODO)  
                            }
                            try
                            {
                                item.stkETADays = int.Parse(xml.GetAttribute("StkETADays"));
                            }
                            catch
                            {  // why is nothing here!??! (TODO) 
                                if (item.stkQtyInStock == 0)
                                    item.stkETADays = 3;
                            } 
                            // ErrorMessage ??
                            // StockType = 'P' ??

                            list.Add(item);
                        }
                    }
                }
                return list;
        }

        public List<Part> GetDeliveredPartsByService(string fromId, string toId, string clientRef, string url)
        {
            List<Part> parts = new List<Part>();
            if (!String.IsNullOrWhiteSpace(url))
            {
                OnlinePartsService.IOnlineSparePartsservice OSP = new OnlinePartsService.IOnlineSparePartsservice();
                if (OSP == null)
                {
                    return parts;
                }
                OSP.Url = url;

                string records = OSP.QueryDeliveryProgressEx(toId.ToUpper(), ospPwd, clientRef);

                byte[] byteArray = Encoding.ASCII.GetBytes(records);
                MemoryStream stream = new MemoryStream(byteArray);

                try
                {
                    XmlTextReader xml = new XmlTextReader(stream);
                    while (xml.Read())
                    {
                        if (xml.AttributeCount != 0)
                        {
                            DateTime dt;
                            int i = 0;
                            int j = 0;
                            Part line = new Part();
                            try
                            { 
                                line.lineNumber = int.Parse(xml.GetAttribute("LineNo")); 
                            }
                            catch
                            { 
                                line.lineNumber = -(i + 1); 
                            }

                            line.partRef = int.Parse(xml.GetAttribute("OrderNumber"));
                            line.stockCode = xml.GetAttribute("StockCode");
                            line.description = xml.GetAttribute("Description");
                            line.saleValue = decimal.Parse(xml.GetAttribute("RetailValue"));
                            line.quantity = int.Parse(xml.GetAttribute("Quant"));
                            line.transactionNo = xml.GetAttribute("TransCode");
                            line.status = int.TryParse(xml.GetAttribute("StatusId").ToString(), out j) ? j : -1;
                            if (line.status == 12)
                            {
                                line.isFitted = "on";
                            }
                            else if (line.status == 13)
                            {
                                line.isReturned = "on";
                            }
                            line.dispatchDate = DateTime.TryParse(xml.GetAttribute("DispatchDate"), out dt) ? (DateTime?)dt : null;
                            line.orderDate = DateTime.TryParse(xml.GetAttribute("OrderDate"), out dt) ? (DateTime?)dt : null;
                            line.courierRef = xml.GetAttribute("CourierRef");
                            line.deliveryNumber = xml.GetAttribute("DeliveryNo");
                            //var list = new List<int> { 12, 13 };
                            //int statusid = 0;
                            //int.TryParse(xml.GetAttribute("StatusId").ToString(), out statusid);
                            //if (!list.Contains(statusid))
                            //{


                            parts.Add(line);
                            //}
                        }
                    }
                }
                catch
                { 
                      // todo... 
                }
            }
            return parts;
        }

        public List<Part> GetOrderedPartsByService(string fromId, string toId, string clientRef, string url)
        {
            List<Part> parts = new List<Part>();
            if (!String.IsNullOrWhiteSpace(url))
            {

                OnlinePartsService.IOnlineSparePartsservice OSP = new OnlinePartsService.IOnlineSparePartsservice();
                if (OSP == null)
                {
                    return parts;
                }
                OSP.Url = url;


                string records = OSP.QueryOrderProgressEx(toId.ToUpper(), ospPwd, clientRef);

                byte[] byteArray = Encoding.ASCII.GetBytes(records);
                MemoryStream stream = new MemoryStream(byteArray);


                try
                {
                    XmlTextReader xml = new XmlTextReader(stream);
                    while (xml.Read())
                    {
                        if (xml.AttributeCount != 0)
                        {
                            DateTime dt;
                            int i = 0;
                            Part line = new Part();
                            try
                            {
                                line.lineNumber = int.Parse(xml.GetAttribute("LineNo"));
                            }
                            catch
                            {
                                line.lineNumber = -(i + 1);
                            }

                            line.partRef = int.Parse(xml.GetAttribute("OrderNumber"));
                            line.stockCode = xml.GetAttribute("StockCode");
                            line.description = xml.GetAttribute("Description");
                            line.saleValue = decimal.Parse(xml.GetAttribute("RetailValue"));
                            line.quantity = int.Parse(xml.GetAttribute("Quant"));
                            line.transactionNo = xml.GetAttribute("TransCode");
                            line.status = int.TryParse(xml.GetAttribute("StatusId").ToString(), out i) ? i : -1;
                            line.dispatchDate = DateTime.TryParse(xml.GetAttribute("DispatchDate"), out dt) ? (DateTime?)dt : null;
                            line.orderDate = DateTime.TryParse(xml.GetAttribute("OrderDate"), out dt) ? (DateTime?)dt : null;
                            line.courierRef = xml.GetAttribute("CourierRef");
                            line.deliveryNumber = xml.GetAttribute("DeliveryNo");
                            parts.Add(line);
                        }
                    }
                }
                catch
                { 
                    // todo ...
                }
            }
            return parts;
        }

        public List<VanPart> GetAllVanParts(string fromId, string userName, string url)
        {
            List<VanPart> list = new List<VanPart>();
            if (!String.IsNullOrWhiteSpace(url))
            {
                OnlinePartsService.IOnlineSparePartsservice OSP = new OnlinePartsService.IOnlineSparePartsservice();
                if (OSP == null)
                {
                    return list;
                }
                OSP.Url = url;

                // Restricted to first 250 records
                string records = OSP.GetAllVanParts(fromId.ToUpper(), ospPwd, userName);

                byte[] byteArray = Encoding.ASCII.GetBytes(records);
                MemoryStream stream = new MemoryStream(byteArray);


                XmlTextReader xml = new XmlTextReader(stream);

                while (xml.Read())
                {
                    if (xml.AttributeCount != 0)
                    {
                        VanPart item = new VanPart();
                        try { item.lineNumber = list.Count() + 1; }
                        catch
                        {  // why is nothing here!??! (TODO)  
                        }
                        try { item.stockCode = xml.GetAttribute("StkCode").Trim(); }
                        catch
                        {  // why is nothing here!??! (TODO) 
                        }
                        try { item.description = xml.GetAttribute("StkDescription").Trim(); }
                        catch
                        {  // why is nothing here!??! (TODO)  
                        }
                        try { item.partRef = int.Parse(xml.GetAttribute("StockID")); }
                        catch
                        {  // why is nothing here!??! (TODO)  
                        }
                        try { item.saleValue = decimal.Parse(xml.GetAttribute("StkSalePrice")); }
                        catch
                        {  // why is nothing here!??! (TODO) 
                        }
                        try { item.costValue = decimal.Parse(xml.GetAttribute("StkCostValue")); }
                        catch
                        {  // why is nothing here!??! (TODO)  
                        }
                        try { item.quantity = int.Parse(xml.GetAttribute("StkQuantInStock")); }
                        catch
                        {  // why is nothing here!??! (TODO) 
                        }
                        list.Add(item);
                    }
                }
            }
            return list;
        }

        public List<VanPart> QueryVanParts(string fromId, string userName, string searchText, string url, int Pagesize)
        {
            List<VanPart> list = new List<VanPart>();
            if (!String.IsNullOrWhiteSpace(url))
            {
                OnlinePartsService.IOnlineSparePartsservice OSP = new OnlinePartsService.IOnlineSparePartsservice();
                if (OSP == null)
                {
                    return list;
                }
                OSP.Url = url;

                // Restricted to first 250 records
                string records = OSP.QueryVanParts(fromId.ToUpper(), ospPwd, "B", searchText + "%", Pagesize, 1, userName);

                byte[] byteArray = Encoding.ASCII.GetBytes(records);
                MemoryStream stream = new MemoryStream(byteArray);


                XmlTextReader xml = new XmlTextReader(stream);

                while (xml.Read())
                {
                    if (xml.AttributeCount != 0)
                    {
                        VanPart item = new VanPart();
                        try
                        {
                            item.lineNumber = list.Count() + 1;
                        }
                        catch
                        {
                            // why is nothing here!??! (TODO)  
                        }
                        try
                        {
                            item.stockCode = xml.GetAttribute("StkCode").Trim();
                        }
                        catch
                        {
                            // why is nothing here!??! (TODO)
                        }
                        try { item.description = xml.GetAttribute("StkDescription").Trim(); }
                        catch
                        {  // why is nothing here!??! (TODO) 
                        }
                        try { item.partRef = int.Parse(xml.GetAttribute("StockID")); }
                        catch
                        {  // why is nothing here!??! (TODO) 
                        }
                        try { item.saleValue = decimal.Parse(xml.GetAttribute("StkSalePrice")); }
                        catch
                        {  // why is nothing here!??! (TODO)
                        }
                        try { item.costValue = decimal.Parse(xml.GetAttribute("StkCostValue")); }
                        catch
                        {
                            // why is nothing here!??! (TODO) 
                        }
                        try { item.quantity = int.Parse(xml.GetAttribute("StkQuantInStock")); }
                        catch
                        {  // why is nothing here!??! (TODO) 
                        }
                        list.Add(item);
                    }
                }
            }
            return list;
        }

        public string QueryOSPVersion(string url)
        {
            string version = "failure";
            try
            {
                if (!String.IsNullOrWhiteSpace(url))
                {
                    OnlinePartsService.IOnlineSparePartsservice OSP = new OnlinePartsService.IOnlineSparePartsservice();
                    OSP.Url = url;

                    version = OSP.Version();
                }
            }
            catch
            {
                version = "osp failure";
            }
            return version;
        }


        public List<Part> GetDeliveredParts(string FromId, string ToId, string[] Clientrefs, string url)
        {
            List<Part> parts = new List<Part>();
            if (!String.IsNullOrWhiteSpace(url))
            {
                OnlinePartsService.IOnlineSparePartsservice OSP = new OnlinePartsService.IOnlineSparePartsservice();
                if (OSP == null)
                {
                    return parts;
                }
                OSP.Url = url;


                string records = OSP.QueryDeliveryProgressBatch(FromId.ToUpper(), ospPwd, Clientrefs);
                logger.Info(string.Format(" GetDeliveryProgressBatch for {0}  ;fromid {1} ; result {2}", string.Join(",", Clientrefs), FromId.ToUpper(), records));
                byte[] byteArray = Encoding.ASCII.GetBytes(records);

                 MemoryStream stream = new MemoryStream(byteArray);

                 try
                 {
                     XmlTextReader xml = new XmlTextReader(stream);
                    while (xml.Read())
                    {
                        if (xml.AttributeCount != 0)
                        {
                            DateTime dt;
                            int i = 0;
                            int j = 0;
                            Part line = new Part();
                            try
                            { 
                                line.lineNumber = int.Parse(xml.GetAttribute("LineNo")); 
                            }
                            catch
                            { 
                                line.lineNumber = -(i + 1); 
                            }
                            line.ServiceId = xml.GetAttribute("ServiceID");
                            line.partRef = int.Parse(xml.GetAttribute("OrderNumber"));
                            line.stockCode = xml.GetAttribute("StockCode");
                            line.description = xml.GetAttribute("Description");
                            line.saleValue = decimal.Parse(xml.GetAttribute("RetailValue"));
                            line.quantity = int.Parse(xml.GetAttribute("Quant"));
                            line.transactionNo = xml.GetAttribute("TransCode");
                            line.status = int.TryParse(xml.GetAttribute("StatusId").ToString(), out j) ? j : -1;
                            if (line.status == 12)
                            {
                                line.isFitted = "on";
                            }
                            else if (line.status == 13)
                            {
                                line.isReturned = "on";
                            }
                            line.dispatchDate = DateTime.TryParse(xml.GetAttribute("DispatchDate"), out dt) ? (DateTime?)dt : null;
                            line.orderDate = DateTime.TryParse(xml.GetAttribute("OrderDate"), out dt) ? (DateTime?)dt : null;
                            line.courierRef = xml.GetAttribute("CourierRef");
                            line.deliveryNumber = xml.GetAttribute("DeliveryNo");
                            // 
                            // var list = new List<int> {12,13};
                            //int statusid=0;
                            //int.TryParse(xml.GetAttribute("StatusId").ToString(), out statusid);
                            //if (!list.Contains(statusid))
                            //{


                            parts.Add(line);
                            //}
                        }
                    }
                 
                     }
                catch (Exception ex)
                {
                    logger.Info(string.Format(" GetDeliveryProgressBatch for {0}  ;fromid {1} ;error : {2}", string.Join(",", Clientrefs), ToId.ToUpper(), ex.Message));
                }
            }
            return parts;

            }
        }
    


    public class OnlineBooking
    {
        public const string bookingPwd = "fecker";

        public List<FollowOnAvailable> FindAvailableFollowOnCall(string fromId, string clientRef, DateTime requestedDate, string username, string password, string url)
        {
            OnlineBookingService.IFzOnlineBookingservice OBS = new IFzOnlineBookingservice();
            OBS.Url = url;

            OnlineBookingService.TFollowOnAvailableRequestDetails request = new TFollowOnAvailableRequestDetails();
            request.UserName = username.ToUpper();
            request.Password = username;
            request.FromID = fromId;
            request.ClientRef = clientRef;
            request.RequestedDate = requestedDate;

            OnlineBookingService.TFollowOnAvailableResponseDetails response =  OBS.FollowOnAvailabilityRequest(request);
            List<FollowOnAvailable> list = new List<FollowOnAvailable>();

            if (response.RequestSuccess)
            {
                int uid = 1;
                foreach (OnlineBookingService.TFollowOnOneAvailableResult availableResult in response.AvailableList)
                {
                    FollowOnAvailable item = new FollowOnAvailable();
                    item.uid = uid++;
                    item.engineerId = availableResult.EngineerID;
                    item.travelDistance = Convert.ToInt32(availableResult.TravelDistance);
                    item.availableDate = availableResult.AvailableDate;
                    list.Add(item);
                }
            }
            else
            {
                throw new Exception(String.Format("FollowOnAvailability {0} : {1}", response.ErrorCode, response.ErrorText));
            }

            return list;
        }

        public FollowOnBookingResult BookFollowOn(string fromId, string clientRef, DateTime requestedDate, string username, string password, int engineerId, string bookingReason, string url)
        {
            OnlineBookingService.IFzOnlineBookingservice OBS = new IFzOnlineBookingservice();
            OBS.Url = url;

            OnlineBookingService.TOnlineBookFollowOnDetails request = new TOnlineBookFollowOnDetails();
            request.UserName = username.ToUpper();
            request.Password = bookingPwd;
            request.FromID = fromId;
            request.ClientRef = clientRef;
            request.SelectedDate = requestedDate;
            request.EngineerID = engineerId;
            // request.Reason = bookingReason;

            OnlineBookingService.TOnlineBookFollowOnResponseDetails response = OBS.BookFollowOn(request);

            FollowOnBookingResult result = new FollowOnBookingResult();
            result.uid = 1;
            result.booked = response.Booked;
            result.clientRef = response.ClientRef;
            if (response.Booked)
            {
                result.bookedDate = requestedDate;
            }
            return result;
        }
    }
}
