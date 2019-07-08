using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mobile.Portal.MSSQL;
using System.Data.SqlClient;
using System.Data;
using Mobile.Portal.Classes;
using System.IO;
using System.Xml;
using Mobile.Portal.Services.SparePartsServer;
using System.Reflection;
using System.Threading;

namespace Mobile.Portal.DAL
{
    public class PartsDataProvider :  DataAccess<CallPart>, Mobile.Portal.DAL.IPartsDataProvider//,DataAccess<StockSearch>  
    {
        public enum StockPartSearchEntity{Stock=1,Model=2,Manufacturer=3,Appliance=4,StockCategory=5};

        public Order OrderPartsForCallId(long _id, string _client, Order order)
        {
            CallDataProvider callsDAL = new CallDataProvider();
            Call call;
            DataSet ds;
            if (_id != 0)
            {
                call = callsDAL.GetById(_id);
                SqlParameter[] parameters = { new SqlParameter("@_ID", _id) };
                ds = MSSQLAccess.SelectStoredProcedure("fz_getOSPURLByCallID", parameters);
            }
            else
            {
                call = new Call();
                SqlParameter[] parameters = { new SqlParameter("@Client_id", _client) };
                ds = MSSQLAccess.SelectStoredProcedure("fz_getOSPURLByClientID", parameters);
            }

            string url = ds.Tables[0].Rows[0]["OSPUrl"].ToString();
            if (url != "")
            {
                IOnlineSparePartsservice OSP = new IOnlineSparePartsservice();
                OSP.Url = url;
              
                string[] orderLines = new string[0];

                foreach (CallPart line in order.Items)
                {
                    string partNote = string.Empty;
                    try
                    {
                        partNote = line.PartNote.Replace(",", string.Empty);
                    }
                    catch { }

                    string str = "'" + line.Id + "'" + "," +     // 1
                        "'" + line.PartReference + "'" + "," +   // 2
                        "'" + line.Quantity + "'" + "," +        // 3
                        "'" + order.CustomerToFit + "'" + "," +  // 4

                        "'" + 0.ToString() + "'" + "," +         // 5 new
                        "'" + string.Empty + "'" + "," +         // 6 new
                        "'" + partNote + "'";                    // 7 new

                    Array.Resize(ref orderLines, orderLines.Length + 1);
                    orderLines[orderLines.Length - 1] = str;
                }

                string records = OSP.OrderMultipleSpareParts(_client, "fecker",
                    "",
                    orderLines,
                    Convert.ToInt32(call.ClientRef),
                    "",
                    order.ToAddress.Contact,
                    order.ToAddress.Address1,
                    order.ToAddress.Address2,
                    order.ToAddress.Address3,
                    order.ToAddress.Address4,
                    order.ToAddress.PostalCode,
                    order.ToAddress.Additional,
                    order.ToAddress.AddressType);

                byte[] byteArray = Encoding.ASCII.GetBytes(records);
                MemoryStream stream = new MemoryStream(byteArray);

                XmlTextReader xml = new XmlTextReader(stream);
                while (xml.Read())
                {
                    if ((xml.Name == "OrderList") && (xml.NodeType == XmlNodeType.Element))
                    {
                        if (xml.GetAttribute("OrderID").ToString() == "~None~" || xml.GetAttribute("OrderID").ToString() == "SOR000000")
                        {
                            order.Status = OrderStatus.Success;
                            if (call.ClientRef != null && call.ClientRef != "" && !call.IsSony)
                            {
                                call.StatusId = 8;
                                callsDAL.Update(call);                                                          
                            }
                        }
                        else
                        {
                            order.Status = OrderStatus.Failure;
                        }
                    }

                    if ((xml.Name == "Order") && (xml.NodeType == XmlNodeType.Element))
                    {
                        CallPart line = order.Items.Find(delegate(CallPart cp) { return cp.PartReference == int.Parse(xml.GetAttribute("StockCode")); });
                        if (line != null)
                        {
                            line.TransactionCode = xml.GetAttribute("ErrDescription");
                        }
                    }
                }
            }
            return order;
        }


        public string OrderPartsForCallIdForSONY(long _id, string _client, Order order)
        {
            CallDataProvider callsDAL = new CallDataProvider();
            string records = string.Empty;
            Call call;
            DataSet ds;

            if (_id != 0)
            {
                call = callsDAL.GetById(_id);
                SqlParameter[] parameters = { new SqlParameter("@_ID", _id) };
                ds = MSSQLAccess.SelectStoredProcedure("fz_getOSPURLByCallID", parameters);
            }
            else
            {
                call = new Call();
                SqlParameter[] parameters = { new SqlParameter("@Client_id", _client) };
                ds = MSSQLAccess.SelectStoredProcedure("fz_getOSPURLByClientID", parameters);
            }


            string url = ds.Tables[0].Rows[0]["OSPUrl"].ToString();

            if (url != "")
            {
                IOnlineSparePartsservice OSP = new IOnlineSparePartsservice();
                OSP.Url = url;

                string[] orderLines = new string[0];

                foreach (CallPart line in order.Items)
                {
                    string partNote = string.Empty;
                    try
                    {
                        partNote = line.PartNote.Replace(",", string.Empty);
                        partNote = line.PartNote.Replace("&", "&amp;");
                        partNote = line.PartNote.Replace("<", "&lt;");
                        partNote = line.PartNote.Replace(">", "&gt;");
                        partNote = line.PartNote.Replace("\"", "&quot;");
                        partNote = line.PartNote.Replace("'","&apos;");

                    }
                    catch { }
                    
                    string str = "'" + line.Id + "'" + "," +
                                 "'" + line.PartReference + "'" + "," +
                                 "'" + line.Quantity + "'" + "," +
                                 "'" + order.CustomerToFit + "'" + "," +  // 4

                                 "'" + 0.ToString() + "'" + "," +         // 5 new
                                 "'" + string.Empty + "'" + "," +         // 6 new
                                 "'" + partNote + "'";                    // 7 new

                    Array.Resize(ref orderLines, orderLines.Length + 1);
                    orderLines[orderLines.Length - 1] = str;
                }

                records = OSP.OrderMultipleVanParts(_client, "fecker", "", orderLines, Convert.ToInt32(call.ClientRef));

                

                //    byte[] byteArray = Encoding.ASCII.GetBytes(records);
                //    MemoryStream stream = new MemoryStream(byteArray);

                //    XmlTextReader xml = new XmlTextReader(stream);
                //    while (xml.Read())
                //    {
                //        if ((xml.Name == "OrderList") && (xml.NodeType == XmlNodeType.Element))
                //        {
                //            if (xml.GetAttribute("OrderID").ToString() == "~None~" || xml.GetAttribute("OrderID").ToString() == "SOR000000")
                //            {
                //                order.Status = OrderStatus.Success;
                //                if (call.ClientRef != null && call.ClientRef != "")
                //                {
                //                    call.StatusId = 8;
                //                    callsDAL.Update(call);
                //                }
                //            }
                //            else
                //            {
                //                order.Status = OrderStatus.Failure;
                //            }
                //        }


                //        if ((xml.Name == "Order") && (xml.NodeType == XmlNodeType.Element))
                //        {
                //            CallPart line = order.Items.Find(delegate(CallPart cp) { return cp.PartReference == int.Parse(xml.GetAttribute("StockCode")); });
                //            if (line != null)
                //            {
                //                line.TransactionCode = xml.GetAttribute("ErrDescription");
                //            }
                //        }
                //    }

            }

            return records;
        }

        public List<CallPart> GetPartsByCallId(long _id)
        {
            CallDataProvider callsDAL = new CallDataProvider();
            Call call = callsDAL.GetById(_id);

            SqlParameter[] parameters = { new SqlParameter("@_ID", _id) };
            DataSet ds = MSSQLAccess.SelectStoredProcedure("fz_getOSPURLByCallID", parameters);

            string url = ds.Tables[0].Rows[0][0].ToString();
            bool usePriceCheck = bool.Parse(ds.Tables[0].Rows[0][2].ToString());

            List<CallPart> parts = new List<CallPart>();
            if (url != "")
            {
                IOnlineSparePartsservice OSP = new IOnlineSparePartsservice();
                OSP.Url = url;

                string records = OSP.QueryOrderProgressEx(call.SaediFromId, "fecker", call.ClientRef);
      
                byte[] byteArray = Encoding.ASCII.GetBytes(records);
                MemoryStream stream = new MemoryStream(byteArray);

                XmlTextReader xml = new XmlTextReader(stream);
                while (xml.Read())
                {
                    int j = 0;
                    if (xml.AttributeCount != 0)
                    {
                        DateTime dt;
                        CallPart line = new CallPart();
                        try
                        { line.LineNo = int.Parse(xml.GetAttribute("LineNo")); }
                        catch
                        { line.LineNo = -(j + 1); }

                        line.CourierReference = "";
                        line.DeliveryNumber = "";
                        line.ReturnDescription = "";
                        line.ReturnReference = "";
                        line.ReturnRequired = false;
                        line.OrderReference = "";
                        line.UsePriceCheck = usePriceCheck;
                        try
                        {
                            try { line.PartReference = xml.GetAttribute("OrderNumber") == null ? 0 : int.Parse(xml.GetAttribute("OrderNumber")); }
                            catch { }
                            try { line.Code = xml.GetAttribute("StockCode") == null ? "" : xml.GetAttribute("StockCode"); }
                            catch { }
                            try { line.Description = xml.GetAttribute("Description") == null ? "" : xml.GetAttribute("Description"); }
                            catch { }
                            try { line.UnitPrice = xml.GetAttribute("RetailValue") == null ? 0 : decimal.Parse(xml.GetAttribute("RetailValue")); }
                            catch { }
                            try { line.Quantity = xml.GetAttribute("Quant") == null ? 0 : int.Parse(xml.GetAttribute("Quant")); }
                            catch { }
                            try { line.TransactionCode = xml.GetAttribute("TransCode") == null ? "" : xml.GetAttribute("ReturnDescription"); }
                            catch { }
                            try { line.Status = xml.GetAttribute("Status") == null ? "" : xml.GetAttribute("ReturnDescription"); }
                            catch { }
                            try { line.StatusID = xml.GetAttribute("StatusId"); }
                            catch { line.StatusID = string.Empty; }
                            try { if (DateTime.TryParse(xml.GetAttribute("DispatchDate"), out dt)) line.DispatchDate = dt; }
                            catch { }
                            try { if (DateTime.TryParse(xml.GetAttribute("OrderDate"), out dt)) line.OrderDate = dt; }
                            catch { }
                            try { line.CourierReference = xml.GetAttribute("CourierRef") == null ? "" : xml.GetAttribute("CourierRef"); }
                            catch { }
                            try { line.DeliveryNumber = xml.GetAttribute("DeliveryNo") == null ? "" : xml.GetAttribute("DeliveryNo"); }
                            catch { }
                            try { line.ReturnDescription = xml.GetAttribute("ReturnDescription") == null ? "" : xml.GetAttribute("ReturnDescription"); }
                            catch { }
                            try { line.ReturnReference = xml.GetAttribute("ReturnReference") == null ? "" : xml.GetAttribute("ReturnReference"); }
                            catch { }
                            try { line.ReturnRequired = xml.GetAttribute("ReturnRequired") == null ? false : bool.Parse(xml.GetAttribute("ReturnRequired")); }
                            catch { }
                            try { line.OrderReference = xml.GetAttribute("OrderReference") == null ? "" : xml.GetAttribute("OrderReference"); }
                            catch { }
                            try { line.UsePriceCheck = usePriceCheck; }
                            catch { }
                        }
                        catch { /* ignore for older web services */ }

                        parts.Add(line);
                        j++;
                    }
                }
            }
            return parts;
        }

        public List<CallPart> FindPartsByCallId(long _id, string _client, string searchText, bool exact)
        {
            List<CallPart> parts = new List<CallPart>();

            CallDataProvider callsDAL = new CallDataProvider();

            Call call;
            DataSet ds;
            if (_id != 0)
            {
                call = callsDAL.GetById(_id);
                SqlParameter[] parameters = { new SqlParameter("@_ID", _id) };
                ds = MSSQLAccess.SelectStoredProcedure("fz_getOSPURLByCallID", parameters);
            }
            else
            {
                call = new Call();
                SqlParameter[] parameters = { new SqlParameter("@Client_id", _client) };
                ds = MSSQLAccess.SelectStoredProcedure("fz_getOSPURLByClientID", parameters);
            }

            string url = ds.Tables[0].Rows[0]["OSPUrl"].ToString();
            bool usePriceCheck = bool.Parse(ds.Tables[0].Rows[0]["UsePriceCheck"].ToString());

            if (url != "")
            {
                IOnlineSparePartsservice OSP = new IOnlineSparePartsservice();
                OSP.Url = url;

                string records = OSP.QuerySpareParts(_client, "fecker", call.ClientRef, exact ? "E" : "B", searchText + "%", 1000000000, 1, "");

                // QuerySpareParts(string UserID, string Passwd, string ServiceID, string QueryField, string QueryValue, int PageSize, int CurrentPage, string clientSaediId)

                try
                {
                    parts = MapFromXML(usePriceCheck, records);
                }
                catch (Exception er)
                {
                    CallPart p = new CallPart();
                    p.ErrorMessage = er.Message + "<br />URL=" + url + "<br />XML=" + records
                                      + "<br />UserID=" + _client
                    + "<br />ServiceID=" + call.ClientRef
                    + "<br />QueryField=" + (exact ? "E" : "B")
                    + "<br />QueryValue=" + searchText + "%"
                    + "<br />PageSize=1000000000"
                    + "<br />CurrentPage=1"
                    + "<br />PageSize=''"; 
                    parts.Clear();
                    parts.Add(p);
                }
            }
            return parts;
        }

        public List<CallPart> FindPartsByModel(long _id)
        {
            Call byId = new CallDataProvider().GetById(_id);
            List<CallPart> list = new List<CallPart>();
            SqlParameter[] parameters = new SqlParameter[] { new SqlParameter("@Client_id", byId.SaediToId) };
            DataSet set = MSSQLAccess.SelectStoredProcedure("fz_getOSPURLByClientID", parameters);
            string str = set.Tables[0].Rows[0]["OSPUrl"].ToString();
            bool usePriceCheck = bool.Parse(set.Tables[0].Rows[0]["UsePriceCheck"].ToString());
            if (str != "")
            {
                string records = new IOnlineSparePartsservice { Url = str }.QuerySpareParts(byId.SaediToId, "fecker", "Model", "M", string.Format("{0}|{0}|{0}", byId.Appliance.ApplianceType.Code, byId.Appliance.Manufacturer.Code, byId.Appliance.Model.Code), 0x3b9aca00, 1, "");
                list = this.MapFromXML(usePriceCheck, records);
            }
            return list;
        }

 


        private List<CallPart> MapFromXML(bool usePriceCheck, string records)
        {
            List<CallPart> parts = new List<CallPart>();
            byte[] byteArray = Encoding.ASCII.GetBytes(records);
            MemoryStream stream = new MemoryStream(byteArray);

            XmlTextReader xml = new XmlTextReader(stream);
            while (xml.Read())
            {
                if (xml.AttributeCount != 0)
                {
                    CallPart part = new CallPart();
                    try { part.LineNo = parts.Count() + 1; }
                    catch { part.LineNo = 0; }
                    try { part.Code = xml.GetAttribute("StkCode"); }
                    catch { part.Code = ""; }
                    try { part.Description = xml.GetAttribute("StkDescription"); }
                    catch { part.Description = ""; }
                    try { part.PartReference = int.Parse(xml.GetAttribute("StockID")); }
                    catch { part.PartReference = 0; }
                    try { part.UnitPrice = decimal.Parse(xml.GetAttribute("StkSalePrice")); }
                    catch { part.UnitPrice = 0; }
                    try { part.Quantity = int.Parse(xml.GetAttribute("StkQuantInStock")); }
                    catch { part.Quantity = 0; }
                    try { part.VanQuantInStock = int.Parse(xml.GetAttribute("VanQuantInStock")); }
                    catch { part.VanQuantInStock = 0; }                   
                    try { part.StockType = xml.GetAttribute("StkType"); }
                    catch { part.StockType = ""; }
                    try { part.UsePriceCheck = usePriceCheck; }
                    catch { part.UsePriceCheck = false; }
                    try { part.ErrorMessage = xml.GetAttribute("ErrorMessage"); }
                    catch { part.ErrorMessage = ""; }
                    try { part.FigNo = xml.GetAttribute("FigNo"); }
                    catch { part.FigNo = ""; }
                    part.CollectionDate = string.Empty;
                    part.Collectionref = string.Empty;

                    parts.Add(part);
                }
            }
            return parts;
        }

        public List<CallPart> FindLocalPartsByCallId(long _id, string searchText)
        {
            CallDataProvider callsDAL = new CallDataProvider();
            Call call = callsDAL.GetById(_id);

            List<CallPart> lines = new List<CallPart>();
            SqlParameter[] parameters1 = { new SqlParameter("@SAEDIID", call.SaediFromId) ,
                                           new SqlParameter("@ClientRef", call.ClientRef),
                                           new SqlParameter("@SearchText", searchText)
                                         };

            DataSet ds = MSSQLAccess.SelectStoredProcedure("fz_getPartsByFreeText", parameters1);

            foreach (DataRow r in ds.Tables[0].Rows)
            {
                CallPart line = new CallPart();
                try { line.Id = lines.Count + 1; }
                catch { line.Id = 0; }
                try { line.Code = r["StkCode"].ToString(); }
                catch { line.Code = ""; }
                try { line.Description = r["StkDescription"].ToString(); }
                catch { line.Description = ""; }
                //try { line.PartReference = int.Parse(r["StkSupplierID"].ToString()); }
                //catch { line.PartReference = 0; }
                try { line.UnitPrice = decimal.Parse(r["StkSalePrice"].ToString()); }
                catch { line.UnitPrice = 0; }
                try { line.Quantity = int.Parse(r["StkSuppAvailable"].ToString()); }
                catch { line.Quantity = 0; }
                line.FigNo = "";
                lines.Add(line);
            }

            return lines;
        }

        public List<CallPart> FindLocalPartsByModel(long _id)
        {
            CallDataProvider callsDAL = new CallDataProvider();
            Call call = callsDAL.GetById(_id);

            List<CallPart> parts = new List<CallPart>();
            SqlParameter[] parameters = { 
                                    new SqlParameter("@SaediSourceId", call.SaediToId),
                                    new SqlParameter("@ApplianceCode", call.Appliance.ApplianceType.Code),
                                    new SqlParameter("@ManufacturerCode", call.Appliance.Manufacturer.Code),
                                    new SqlParameter("@ModelCode", call.Appliance.Model.Code)
                                };
            parts = MapRows(MSSQLAccess.SelectStoredProcedure("fz_getBOMByAppliance", parameters));
            return parts;
        }

       

        public override CallPart MapDataToClass(DataRow row)
        {
            CallPart part = new CallPart();
            if(row.Table.Columns.Contains("SAEDISourceID"))
            {  if (!row["SAEDISourceID"].ToString().Contains("SONY3C") || !row["SAEDISourceID"].ToString().Contains("SONYCC"))
            { 
                part.Id = int.Parse(row["_id"].ToString());
            part.LineNo = 0;
            part.Code = row["StockCode"].ToString();
            part.Description = row["Description"].ToString();
            part.PartReference = int.Parse(MapCell(row, "StockId", 0).ToString());
            part.UnitPrice = decimal.Parse(MapCell(row, "SalePrice", 0).ToString());
            part.Quantity = int.Parse(MapCell(row, "Quantity", 0).ToString());
            part.FigNo = row["ItemNumber"].ToString();
            }}

            else
            {
                // Mandatory 
                part.Id = int.Parse(row["Id"].ToString());
                part.SAEDIFromID = row["SAEDIFromID"].ToString();
                part.SAEDICallRef = row["SAEDICallRef"].ToString();
                part.Code = row["CodeID"].ToString();
                part.PartReference = int.Parse(row["OrderNumber"].ToString());
                part.StatusID = row["StatusID"].ToString();
                // Not Mandatory
                part.OrderReference = row["OrderReference"] == null ? string.Empty : row["OrderReference"].ToString();
                part.ReturnReference = row["ReturnReference"] == null ? string.Empty : row["ReturnReference"].ToString();
                part.ReturnRequired = row["ReturnRequired"].ToString() == "1" ? true : false;
                part.ReturnDescription = row["ReturnDescription"] == null ? string.Empty : row["ReturnDescription"].ToString();
                part.DeliveryNumber = row["DeliveryNumber"] == null ? string.Empty : row["DeliveryNumber"].ToString();
                part.CourierReference = row["CourierReference"] == null ? string.Empty : row["CourierReference"].ToString();
                part.Description = row["PartDescription"] == null ? string.Empty : row["PartDescription"].ToString();
                part.UnitPrice = row["UnitPrice"] == null ? 0 : decimal.Parse(MapCell(row, "UnitPrice", 0).ToString());
                part.Quantity = row["Quantity"] == null ? 0 : int.Parse(MapCell(row, "Quantity", 0).ToString());
                part.TransactionCode = row["TransactionCode"] == null ? string.Empty : row["TransactionCode"].ToString();
                part.Status = part.StatusID.ToUpper() == "V" ? "Stock Part" : (row["StatusTitle"] == null ? string.Empty : row["StatusTitle"].ToString());
                part.IsAllocated = row["Allocated"].ToString() == "1" ? true : false;
                part.IsEstimate = row["Estimated"].ToString() == "1" ? true : false;
                part.IsFitted = row["Fitted"].ToString() == "1" ? true : false;
                // part.IsPrimary = row["Primary"].ToString() == "1" ? true : false;
                part.IsPartConsumptionDone = row["PartConsumption"].ToString() == "1" ? true : false;
                part.IsStock = part.Status.ToUpper() == "V" ? true : false;
                try { part.OrderDate = (DateTime)row["OrderDate"]; }
                catch { };
                try { part.DispatchDate = (DateTime)row["DispatchDate"]; }
                catch { };
                try { part.RmaDocumentUrl = row["RmaDocumentUrl"].ToString(); }
                catch { };
                try { part.IsRmaDone = row["RmaDone"].ToString() == "1" ? true : false; }
                catch
                {
                    part.IsRmaDone = false;
                };
                try { part.INPUTson = row["INPUTson"].ToString(); }
                catch
                {
                    part.INPUTson = string.Empty;
                };
                try { part.IsBulletin = row["IsBulletin"].ToString() == "1" ? true : false; }
                catch
                {
                    part.IsBulletin = false;
                };

                try { part.IsPrimary = row["IsPrimary"].ToString() == "1" ? true : false; }
                catch
                {
                    part.IsPrimary = false;
                };

                try { part.ValidationStatus = row["ValidationStatus"].ToString(); }
                catch
                {
                    part.ValidationStatus = string.Empty;
                };
                try { part.ShipmentStatus = row["ShipmentStatus"].ToString(); }
                catch
                {
                    part.ShipmentStatus = string.Empty;
                };
                try { part.WarrantyStatus = row["WarrantyStatus"].ToString(); }
                catch
                {
                    part.WarrantyStatus = string.Empty;
                };

                try { part.INPUTascMaterialId = row["INPUTascMaterialId"].ToString(); }
                catch
                {
                    part.INPUTascMaterialId = string.Empty;
                };
                try { part.Collectionref = row["Collectionref"].ToString(); }
                catch
                {
                    part.Collectionref = string.Empty;
                };
                try { part.CollectionDate =Convert.ToDateTime( row["CollectionDate"].ToString()).ToShortDateString(); }
                catch
                {
                    part.CollectionDate = string.Empty;
                };
                part.LabelUrl = row["LabelUrl"].ToString();
                part.ConNoteUrl = row["ConNoteUrl"].ToString();
                part.ConsignmentNo = row["ConsignmentNo"].ToString();
                part.BookingUniqueNumber = row["BookingUniqueNumber"].ToString();
                part.INPUT_CourierID = row["INPUT_CourierID"].ToString();
            }
                      
            return part;
        }

        public List<CallPart> GetSAEDIPartsByCall(string saediId, string saediCallRef)
        {
            SqlParameter[] parameters = { 
                                          MSSQLAccess.BuildParameter("@saediID", SqlDbType.VarChar, saediId.Trim()),
                                          MSSQLAccess.BuildParameter("@saediCallRef", SqlDbType.VarChar, saediCallRef.Trim())
                                        };

            return SelectStoredProc("fz_GetSAEDIPartsByCall", parameters);
        }

        public List<CallPart> GetSAEDIPartByOrderNumber(int orderNumber, string statudID)
        {
            SqlParameter[] parameters = { 
                                          MSSQLAccess.BuildParameter("@OrderNumber", SqlDbType.Int, 15, orderNumber),
                                          MSSQLAccess.BuildParameter("@StatusID", SqlDbType.VarChar, statudID.Trim())
                                        };

            return SelectStoredProc("fz_GetSAEDIPartByOrderNumber", parameters);
        }

        public void DeleteSAEDIPart(CallPart part)
        {
            SqlParameter[] parameters = { 
                                          MSSQLAccess.BuildParameter("@OrderNumber", SqlDbType.Int, 15, part.PartReference),
                                          MSSQLAccess.BuildParameter("@StatusID", SqlDbType.VarChar, part.StatusID.Trim())
                                        };
            int rows;
            ExecuteStoredProc("fz_DeleteSAEDIPart", parameters, out rows);
        }

        public void UpdateSAEDIPart(CallPart part)
        {
            SqlParameter[] parameters = { 
                                          MSSQLAccess.BuildParameter("@SAEDIFromID", SqlDbType.VarChar, part.SAEDIFromID.Trim()),
                                          MSSQLAccess.BuildParameter("@SAEDICallRef", SqlDbType.VarChar, part.SAEDICallRef.Trim()),
                                          MSSQLAccess.BuildParameter("@CodeID", SqlDbType.VarChar, part.Code),

                                          MSSQLAccess.BuildParameter("@OrderReference", SqlDbType.VarChar, part.OrderReference.Trim()),
                                          MSSQLAccess.BuildParameter("@ReturnReference", SqlDbType.VarChar, part.ReturnReference.Trim()),
                                          MSSQLAccess.BuildParameter("@ReturnRequired", SqlDbType.VarChar, part.ReturnRequired == true ? "1" : "0"),
                                          MSSQLAccess.BuildParameter("@ReturnDescription", SqlDbType.VarChar, part.ReturnDescription.Trim()),
                                          MSSQLAccess.BuildParameter("@DeliveryNumber", SqlDbType.VarChar, part.DeliveryNumber.Trim()),
                                          MSSQLAccess.BuildParameter("@CourierReference", SqlDbType.VarChar, part.CourierReference.Trim()),
                                          
                                          MSSQLAccess.BuildParameter("@PartDescription", SqlDbType.Text, part.Description), 
                                          MSSQLAccess.BuildParameter("@OrderNumber", SqlDbType.Int, 15, part.PartReference),
                                          MSSQLAccess.BuildParameter("@UnitPrice", SqlDbType.Decimal, 18, part.UnitPrice),
                                          MSSQLAccess.BuildParameter("@Quantity", SqlDbType.Int, 4, part.Quantity),
                                          MSSQLAccess.BuildParameter("@TransactionCode", SqlDbType.VarChar, part.TransactionCode),
                                          MSSQLAccess.BuildParameter("@StatusTitle", SqlDbType.VarChar, part.Status),
                                          MSSQLAccess.BuildParameter("@StatusID", SqlDbType.VarChar, part.StatusID),
                                          MSSQLAccess.BuildParameter("@Allocated", SqlDbType.VarChar, part.IsAllocated == true ? "1" : "0"),
                                          MSSQLAccess.BuildParameter("@Estimated", SqlDbType.VarChar, part.IsEstimate == true ? "1" : "0"),
                                          MSSQLAccess.BuildParameter("@Fitted", SqlDbType.VarChar, part.IsFitted == true ? "1" : "0"),
                                          MSSQLAccess.BuildParameter("@Primary", SqlDbType.VarChar, part.IsPrimary == true ? "1" : "0"),
                                          MSSQLAccess.BuildParameter("@PartConsumption", SqlDbType.VarChar, part.IsPartConsumptionDone == true ? "1" : "0"),
                                          MSSQLAccess.BuildParameter("@OrderDate", SqlDbType.DateTime, part.OrderDate),
                                          MSSQLAccess.BuildParameter("@DispatchDate", SqlDbType.DateTime, part.DispatchDate), 
                                         // MSSQLAccess.BuildParameter("@RetCourierRef", SqlDbType.VarChar, part.RetCourierRef),             
                                        };
            int rows;
            ExecuteStoredProc("fz_UpdateSAEDIPart", parameters, out rows);
        }

        public void InsertSAEDIPart(CallPart part)
        {
            SqlParameter[] parameters = { 
                                          MSSQLAccess.BuildParameter("@SAEDIFromID", SqlDbType.VarChar, part.SAEDIFromID != null ? part.SAEDIFromID.Trim() : string.Empty),
                                          MSSQLAccess.BuildParameter("@SAEDICallRef", SqlDbType.VarChar, part.SAEDICallRef != null ? part.SAEDICallRef.Trim() : string.Empty),
                                          MSSQLAccess.BuildParameter("@CodeID", SqlDbType.VarChar, part.Code != null ? part.Code.Trim() : string.Empty),

                                          MSSQLAccess.BuildParameter("@OrderReference", SqlDbType.VarChar, part.OrderReference != null ? part.OrderReference.Trim() : string.Empty),
                                          MSSQLAccess.BuildParameter("@ReturnReference", SqlDbType.VarChar, part.ReturnReference != null ? part.ReturnReference.Trim() : string.Empty),
                                          MSSQLAccess.BuildParameter("@ReturnRequired", SqlDbType.VarChar, part.ReturnRequired == true ? "1" : "0"),
                                          MSSQLAccess.BuildParameter("@ReturnDescription", SqlDbType.VarChar, part.ReturnDescription != null ? part.ReturnDescription.Trim() : string.Empty),
                                          MSSQLAccess.BuildParameter("@DeliveryNumber", SqlDbType.VarChar, part.DeliveryNumber != null ? part.DeliveryNumber.Trim() : string.Empty),
                                          MSSQLAccess.BuildParameter("@CourierReference", SqlDbType.VarChar, part.CourierReference != null ? part.CourierReference.Trim() : string.Empty),

                                          MSSQLAccess.BuildParameter("@PartDescription", SqlDbType.Text, part.Description), 
                                          MSSQLAccess.BuildParameter("@OrderNumber", SqlDbType.Int, 15, part.PartReference),
                                          MSSQLAccess.BuildParameter("@UnitPrice", SqlDbType.Decimal, 18, part.UnitPrice),
                                          MSSQLAccess.BuildParameter("@Quantity", SqlDbType.Int, 4, part.Quantity),
                                          MSSQLAccess.BuildParameter("@TransactionCode", SqlDbType.VarChar, part.TransactionCode),
                                          MSSQLAccess.BuildParameter("@StatusTitle", SqlDbType.VarChar, part.Status),
                                          MSSQLAccess.BuildParameter("@StatusID", SqlDbType.VarChar, part.StatusID),
                                          MSSQLAccess.BuildParameter("@Allocated", SqlDbType.VarChar, part.IsAllocated == true ? "1" : "0"),
                                          MSSQLAccess.BuildParameter("@Estimated", SqlDbType.VarChar, part.IsEstimate == true ? "1" : "0"),
                                          MSSQLAccess.BuildParameter("@Fitted", SqlDbType.VarChar, part.IsFitted == true ? "1" : "0"),
                                          MSSQLAccess.BuildParameter("@Primary", SqlDbType.VarChar, part.IsPrimary == true ? "1" : "0"),
                                          MSSQLAccess.BuildParameter("@PartConsumption", SqlDbType.VarChar, part.IsPartConsumptionDone == true ? "1" : "0"),
                                          MSSQLAccess.BuildParameter("@OrderDate", SqlDbType.DateTime, part.OrderDate),
                                          MSSQLAccess.BuildParameter("@DispatchDate", SqlDbType.DateTime, part.DispatchDate),                                                        
                                        };
            int rows;
            ExecuteStoredProc("fz_InsertSAEDIPart", parameters, out rows);
        }

        public bool AcceptByCallAndPartId(long _callId, int _partId)
        {
            return ChangePartStatus(_callId, _partId, 4);
        }

        public bool RejectByCallAndPartId(long _callId, int _partId)
        {
            return ChangePartStatus(_callId, _partId, 3);
        }

        public bool UsesPartOrdering(long _callId)
        {
            SqlParameter[] parametersURL = { new SqlParameter("@_ID", _callId) };
            DataSet dsURL = MSSQLAccess.SelectStoredProcedure("fz_getOSPURLByCallID", parametersURL);
            if ((dsURL.Tables[0].Rows.Count != 0) && (dsURL.Tables[0].Rows[0][0] != null) && (dsURL.Tables[0].Rows[0][0].ToString() != ""))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private static bool ChangePartStatus(long _callId, int _partId, int _statusId)
        {
            bool retVal = false;
            CallDataProvider callsDAL = new CallDataProvider();
            Call call = callsDAL.GetById(_callId);


            SqlParameter[] parameters = { new SqlParameter("@_ID", _callId) };
            DataSet ds = MSSQLAccess.SelectStoredProcedure("fz_getOSPURLByCallID", parameters);

            string url = ds.Tables[0].Rows[0][0].ToString();

            List<CallPart> parts = new List<CallPart>();
            if (url != "")
            {

                IOnlineSparePartsservice OSP = new IOnlineSparePartsservice();
                OSP.Url = url;
                string boolStr = OSP.ChangeSparePartStatus(call.SaediFromId, "fecker", _partId, Convert.ToInt32(call.ClientRef), _statusId);
                if (boolStr == "True")
                {
                    retVal = true;
                }

            }

            return retVal;
        }

        public List<PartImage> GetPartImageLinks(int _stockId)
        {
            SqlParameter[] parameters = { 
                                    new SqlParameter("@StockId", _stockId),
                                    };
            return populateImageLinks(MSSQLAccess.SelectStoredProcedure("fz_getImageLinksByStockId", parameters));
        }

        public List<PartImage> populateImageLinks(DataSet ds)
        {
            List<PartImage> list = new List<PartImage>();
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                char[] delimiters = new char[] { '\r', '\n' };
                string fn = ds.Tables[0].Rows[i]["FileNames"].ToString();
                String[] filenames = fn.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
                foreach (string s in filenames)
                {
                    PartImage line = new PartImage();
                    line.StockId = int.Parse(ds.Tables[0].Rows[i]["StockId"].ToString());
                    line.FileName = Path.GetFileName(s);
                    list.Add(line);
                }
            }
            return list;
        }

        public bool ReturnByCallAndPartId(long _callId, int _partId, string _returnReference, string note)
        {
            bool retVal = false;
            CallDataProvider callsDAL = new CallDataProvider();
            Call call = callsDAL.GetById(_callId);

            SqlParameter[] parameters = { new SqlParameter("@_ID", _callId) };
            DataSet ds = MSSQLAccess.SelectStoredProcedure("fz_getOSPURLByCallID", parameters);

            string url = ds.Tables[0].Rows[0][0].ToString();

            List<CallPart> parts = new List<CallPart>();
            if (url != "")
            {
                IOnlineSparePartsservice OSP = new IOnlineSparePartsservice();
                OSP.Url = url;
                // OSP.Url = "http://connect.fieldengineer.co.uk/PartsServer/SparePartsServerCGIPilot.exe/soap/IOnlineSpareParts"; // TEST
                string boolStr = OSP.ChangeSparePartReturn(call.SaediFromId, "fecker", _partId, Convert.ToInt32(call.ClientRef), _returnReference, note);
                if (boolStr == "True")
                {
                    retVal = true;
                }
            }

            return retVal;
        }

        public bool ReturnByCallAndPartIdForSONY(long _callId, int _partId, string _returnReference, string note)
        {
            bool retVal = false;
            CallDataProvider callsDAL = new CallDataProvider();
            Call call = callsDAL.GetById(_callId);

            //SqlParameter[] parameters = { new SqlParameter("@_ID", _callId) };
            //DataSet ds = MSSQLAccess.SelectStoredProcedure("fz_getOSPURLByCallID", parameters);
            //string url = ds.Tables[0].Rows[0][0].ToString();

            OSPRefDataProvider OSPRefBLL = new OSPRefDataProvider();
            List<OSPRefs> OSPRefList = OSPRefBLL.GetOSPRefByCallID(call.Id.ToString());
            OSPRefs OSPRef = new OSPRefs();
            if (OSPRefList.Count > 0)
                OSPRef = OSPRefList[0];
            else
                OSPRef.OSPUrl = string.Empty;
            string url = OSPRef.OSPUrl;
            
            List<CallPart> parts = new List<CallPart>();
            if (url != "")
            {
                IOnlineSparePartsservice OSP = new IOnlineSparePartsservice();
                OSP.Url = url;
                // OSP.Url = "http://connect.fieldengineer.co.uk/PartsServer/SparePartsServerCGIPilot.exe/soap/IOnlineSpareParts"; // TEST
                // string boolStr = OSP.ChangeSparePartReturn(call.SaediFromId, "fecker", _partId, Convert.ToInt32(call.ClientRef), _returnReference);
                string boolStr = OSP.ChangeVanPartReturn(call.SaediFromId, "fecker", _partId, Convert.ToInt32(call.ClientRef), _returnReference, note);

                if (boolStr == "True")
                {
                    retVal = true;
                }
            }
            return retVal;
        }

        public string DeleteVanPartForSONY(long _callId, int _partId)
        {
            CallDataProvider callsDAL = new CallDataProvider();
            Call call = callsDAL.GetById(_callId);

            OSPRefDataProvider OSPRefBLL = new OSPRefDataProvider();
            List<OSPRefs> OSPRefList = OSPRefBLL.GetOSPRefByCallID(call.Id.ToString());
            OSPRefs OSPRef = new OSPRefs();
            if (OSPRefList.Count > 0)
                OSPRef = OSPRefList[0];
            else
                OSPRef.OSPUrl = string.Empty;

            string url = OSPRef.OSPUrl;
            string result = string.Empty;

            List<CallPart> parts = new List<CallPart>();
            if (url != "")
            {
                IOnlineSparePartsservice OSP = new IOnlineSparePartsservice();
                OSP.Url = url;
                result = OSP.DeleteVanPart(call.SaediFromId, "fecker", _partId, Convert.ToInt32(call.ClientRef));
            }
            return result;
        }

        public int UpdateReturnReferenceForSONY(string password, string saediFromID, string supplierReference, string returnReference, string note)
        {
            int retVal = 0;

            OSPRefDataProvider OSPRefBLL = new OSPRefDataProvider();
            List<OSPRefs> OSPRefList = OSPRefBLL.GetOSPRefByClientID(saediFromID);
            OSPRefs OSPRef = new OSPRefs();
            if (OSPRefList.Count > 0)
                OSPRef = OSPRefList[0];
            else
                OSPRef.OSPUrl = string.Empty;

            IOnlineSparePartsservice OSP = new IOnlineSparePartsservice();
            OSP.Url = OSPRef.OSPUrl;
            // OSP.Url = "http://connect.fieldengineer.co.uk/PartsServer/SparePartsServerCGIPilot.exe/soap/IOnlineSpareParts"; // TEST
            retVal = OSP.UpdateReturnReference(saediFromID, password, supplierReference, returnReference, note);       

            return retVal;
        }

        public string ChangeSparePartStatus(string saediID, int partOrderNumber, int serviceID, int statusID)
        {
            OSPRefDataProvider OSPRefBLL = new OSPRefDataProvider();
            List<OSPRefs> OSPRefList = OSPRefBLL.GetOSPRefByClientID(saediID);
            OSPRefs OSPRef = new OSPRefs();
            if (OSPRefList.Count > 0)
                OSPRef = OSPRefList[0];
            else
                OSPRef.OSPUrl = string.Empty;

            IOnlineSparePartsservice OSP = new IOnlineSparePartsservice();
            OSP.Url = OSPRef.OSPUrl;
            string result = string.Empty; 
            result = OSP.ChangeSparePartStatus(saediID, "fecker", partOrderNumber, serviceID, statusID);
            return result;
        }
     
        public void UpdateStockSearchTermsLog(string StockSearchTermsid,string xmllog)
         {
            SqlParameter[] parameters = { new SqlParameter("@_id", int.Parse(StockSearchTermsid)) ,
                                          
                                           new SqlParameter("@XMLlog", xmllog) };
             DataSet set =   (MSSQLAccess.SelectStoredProcedure("Fz_UpdateStockSearchTermsLog", parameters));
         }

        //public DataSet SearchPartsByText(string searchText, string SaediFromIDId, string SAEDIID,string orginalText)
        //{
        //    DataSet ds = new DataSet();
        //    SqlParameter[] parameters = { new SqlParameter("@SaediFromId", SaediFromIDId) ,
        //                                    new SqlParameter("@SAEDItoID", SAEDIID) ,
        //                                   new SqlParameter("@SearckKey", orginalText) , 
        //                                   new SqlParameter("@XMLlog", "") };
        //    int Searchtermswebid =(int)( (MSSQLAccess.SelectStoredProcedure("Fz_InsertStockSearchTermsLog", parameters)).Tables[0]).Rows[0][0];
        //    try
        //    {
        //       // string SAEDIID = "JTM498";
        //        DataTable dt = new DataTable();
        //        dt.Columns.Add(new DataColumn("StockId",typeof(Int32)));
        //        dt.Columns.Add(new DataColumn("StkCode"));
        //        dt.Columns.Add(new DataColumn("StkDescription"));
        //        dt.Columns.Add(new DataColumn("StockPrice"));
        //        dt.Columns.Add(new DataColumn("StockSearchEntitiesWebWord"));
        //        dt.Columns.Add(new DataColumn("StockSearchEntitiesWebEntity"));
        //        dt.Columns.Add(new DataColumn("modelcode"));
        //        dt.Columns.Add(new DataColumn("modeldescription"));
        //        dt.Columns.Add(new DataColumn("modellink"));
        //        dt.Columns.Add(new DataColumn("ModelId"));
        //        dt.Columns.Add(new DataColumn("manufactid")); 
        //        dt.Columns.Add(new DataColumn("ManufacturerCode"));
        //        dt.Columns.Add(new DataColumn("ManufactDesc"));
        //        dt.Columns.Add(new DataColumn("ApplianceTypeID"));
        //        dt.Columns.Add(new DataColumn("ApplianceCode"));
        //        dt.Columns.Add(new DataColumn("ApplianceDesc"));
        //        dt.Columns.Add(new DataColumn("StkCategorydescription"));
        //        dt.Columns.Add(new DataColumn("StkCategoryId"));
        //        dt.Columns.Add(new DataColumn("StockSearchEntitiesWebScore", typeof(Int32)));
        //        dt.Columns.Add(new DataColumn("StockSearchentitiesWebPlacing", typeof(Int32)));
            
        //        string[] searchTextArr = searchText.Split(' ');
        //    //    DataTable 
        //      //  Thread t1 = new Thread(delegate() { SearchResult(dt, "fz_getPartsByStockCode", SAEDIID, searchTextArr, (int)StockPartSearchEntity.Stock); });

        //        //foreach (string searchtext in searchTextArr)
        //        //{
                   
        //        //   dt = SearchResult(dt,"fz_getPartsByStockCode",SAEDIID,searchtext,(int) StockPartSearchEntity.Stock);
        //        //   dt = SearchResult(dt, "fz_getPartsByStockDescription", SAEDIID, searchtext, (int)StockPartSearchEntity.Stock);
        //        //   dt = SearchResult(dt, "fz_getPartsByModelCode", SAEDIID, searchtext, (int)StockPartSearchEntity.Model); //(MSSQLAccess.SelectStoredProcedure("fz_getPartsByModelCode", (SqlParameter[])(parameters.Clone()))).Tables[0];
        //        //   dt = SearchResult(dt, "fz_getPartsByManuFacture", SAEDIID, searchtext, (int)StockPartSearchEntity.Manufacturer);
        //        //   dt = SearchResult(dt, "fz_getPartsByAppliance", SAEDIID, searchtext, (int)StockPartSearchEntity.Appliance); // (MSSQLAccess.SelectStoredProcedure("fz_getPartsByAppliance", parameters)).Tables[0];
        //        //   dt = SearchResult(dt, "fz_getPartsByStockCategory", SAEDIID, searchtext, (int)StockPartSearchEntity.StockCategory);//"StockCategory");
        //        //}
        //        DataColumn Searchtermswebcolumn = new DataColumn("StocksearchTermsWebID", typeof(System.Int32));
        //           Searchtermswebcolumn.DefaultValue = Searchtermswebid;
        //           dt.Columns.Add(Searchtermswebcolumn);

        //           //DataTable dtscoreGrouped = GroupBy(new string[] { "StockId", "StkCode", "StkDescription", "StockPrice" }, "StockSearchEntitiesWebScore", dt);
        //           //ds.Tables.Add(dtscoreGrouped);
        //           ds.Tables.Add(SUMDuplicatesRecords(dt)); 
        //        //StocksearchentitiesWebBulkCopy(dt, Searchtermswebid);
        //        return ds;
        //    }
        //    catch (Exception ex)
        //    {
        //        string message = ex.Message;
        //        return ds;
        //    }
           
        //}

        public DataTable GroupBy(string[] i_sGroupByColumn, string i_sAggregateColumn, DataTable i_dSourceTable)
        {

            DataView dv = new DataView(i_dSourceTable);
            dv.RowFilter = "StockSearchEntitiesWebEntity in (1,4,5) ";
            //dv.RowFilter = "StockSearchEntitiesWebEntity=4 ";
            //dv.RowFilter = "StockSearchEntitiesWebEntity=5 ";
            //getting distinct values for group column
            DataTable dtGroup = dv.ToTable(true, i_sGroupByColumn);// new string[] { i_sGroupByColumn });

            //adding column for the row count
            dtGroup.Columns.Add("TotalScore", typeof(int));
            dtGroup.Columns.Add("Count", typeof(int));
            //looping thru distinct values for the group, counting
            foreach (DataRow dr in dtGroup.Rows)
            {
                dr["TotalScore"] = i_dSourceTable.Compute("Sum(" + i_sAggregateColumn + ")", i_sGroupByColumn[0] + " = '" + dr[i_sGroupByColumn[0]] + "' AND " + i_sGroupByColumn[1] + " = '" + dr[i_sGroupByColumn[1]] + "' AND " + i_sGroupByColumn[2] + " = '" + dr[i_sGroupByColumn[2]] + "' AND " + i_sGroupByColumn[3] + " = '" + dr[i_sGroupByColumn[3]] + "'");
                dr["Count"] = i_dSourceTable.Compute("Count(" + i_sAggregateColumn + ")", i_sGroupByColumn[0] + " = '" + dr[i_sGroupByColumn[0]] + "' AND " + i_sGroupByColumn[1] + " = '" + dr[i_sGroupByColumn[1]] + "' AND " + i_sGroupByColumn[2] + " = '" + dr[i_sGroupByColumn[2]] + "' AND " + i_sGroupByColumn[3] + " = '" + dr[i_sGroupByColumn[3]] + "'");
            }
            //returning grouped/counted result
            return dtGroup;
        }
        public  void StocksearchentitiesWebBulkCopy(DataSet dt,int Searchtermswebid)
        {
            try
            {
                DataTable StockSearchEntititsWeb = new DataTable();
                StockSearchEntititsWeb.Columns.Add("StocksearchTermsWebID", typeof(int));
                StockSearchEntititsWeb.Columns.Add("StockSearchEntitiesWebWord", typeof(string));
                StockSearchEntititsWeb.Columns.Add("StockSearchentitiesWebEntity", typeof(int));
                StockSearchEntititsWeb.Columns.Add("StockSearchentitiesWebRecord", typeof(int));
                StockSearchEntititsWeb.Columns.Add("StockSearchentitiesWebScore", typeof(int));
                StockSearchEntititsWeb.Columns.Add("StockSearchentitiesWebPlacing", typeof(int));


                foreach (DataRow row in dt.Tables["ScoreGrouped"].Rows)
                {
                    DataTable dtFiltered = (dt.Tables["Detailresult"].AsEnumerable()
                 .Where(row1 => row1.Field<Int32>("StockId") == row.Field<Int32>("StockId"))
                 .CopyToDataTable());

                    DataRow rowFiltered = dtFiltered.Rows[0];
                    int count = dtFiltered.Rows.Count;
                    StockSearchEntititsWeb.Rows.Add(Searchtermswebid, ( rowFiltered["StockSearchEntitiesWebWord"].ToString().Length>50)?rowFiltered["StockSearchEntitiesWebWord"].ToString().Substring(0,50):rowFiltered["StockSearchEntitiesWebWord"], rowFiltered["StockSearchentitiesWebEntity"], count, row["TotalScore"], dt.Tables["ScoreGrouped"].Rows.IndexOf(row) + 1);// row["StockSearchentitiesWebPlacing"]);
                }

                DataTable StockSearchEntitiesWebDT = new DataTable();

                // SqlParameter[] parameters = { new SqlParameter("@StockSearchEntititsWeb", SqlDbType.Structured) };
                SqlParameter[] parameters = { MSSQLAccess.BuildParameter("@StockSearchEntititsWebType", SqlDbType.Structured, sizeof(Int16), StockSearchEntititsWeb) };
                int rowsAffected;
                int record = ExecuteStoredProc("Fz_InsertStockSearchEntititsWeb", parameters, out rowsAffected);
            }
            catch(Exception ex)
            {
            }
        }

        public DataTable ToDataTable<T>(IEnumerable<T> varlist)
        {
            DataTable dtReturn = new DataTable();

            // column names
            PropertyInfo[] oProps = null;

            if (varlist == null) return dtReturn;

            foreach (T rec in varlist)
            {
                // Use reflection to get property names, to create table, Only first time, others
                // will follow
                if (oProps == null)
                {
                    oProps = ((Type)rec.GetType()).GetProperties();
                    foreach (PropertyInfo pi in oProps)
                    {
                        Type colType = pi.PropertyType;

                        if ((colType.IsGenericType) && (colType.GetGenericTypeDefinition() == typeof(Nullable<>)))
                        {
                            colType = colType.GetGenericArguments()[0];
                        }

                        dtReturn.Columns.Add(new DataColumn(pi.Name, colType));
                    }
                }

                DataRow dr = dtReturn.NewRow();

                foreach (PropertyInfo pi in oProps)
                {
                    dr[pi.Name] = pi.GetValue(rec, null) == null ? DBNull.Value : pi.GetValue(rec, null);
                }
                dtReturn.Rows.Add(dr);
            }
            return dtReturn;
        }

        private DataTable SUMDuplicatesRecords(DataTable dt)
        {
            var groupQuery = from r in dt.AsEnumerable()
                             group r by new
                             {
                                 StockId = r["StockId"],
                                 StockSearchEntitiesWebEntity = r["StockSearchEntitiesWebEntity"],
                                 StkCode = r["StkCode"],
                                 StkDescription = r["StkDescription"],
                                 StockPrice = r["StockPrice"],
                                 StockSearchEntitiesWebWord = r["StockSearchEntitiesWebWord"],
                                 modelcode = r["modelcode"],
                                 modeldescription = r["modeldescription"],
                                 modellink = r["modellink"],
                                 ModelId = r["Modelid"],
                                 manufactid = r["manufactid"],
                                 ManufacturerCode = r["ManufacturerCode"],
                                 ManufactDesc = r["ManufactDesc"],
                                 ApplianceTypeID = r["ApplianceTypeID"],
                                 ApplianceCode = r["ApplianceCode"],
                                 ApplianceDesc = r["ApplianceDesc"],
                                 StkCategorydescription = r["StkCategorydescription"],
                                 StkCategoryId = r["StkCategoryId"],
                                 StocksearchTermsWebID = r["StocksearchTermsWebID"],
                                 StockSearchentitiesWebPlacing = r["StockSearchentitiesWebPlacing"]
                             }
                                 into groupedTable
                                 select new
                                 {
                                     StockId = groupedTable.Key.StockId,
                                     StockSearchEntitiesWebEntity = groupedTable.Key.StockSearchEntitiesWebEntity,
                                     StockSearchEntitiesWebScore = groupedTable.Sum(r => r.Field<int>("StockSearchEntitiesWebScore")),
                                     StkCode = groupedTable.Key.StkCode,
                                     StkDescription =groupedTable.Key.StkDescription,
                                     StockPrice = groupedTable.Key.StockPrice,
                                     StockSearchEntitiesWebWord = groupedTable.Key.StockSearchEntitiesWebWord,
                                     modelcode =groupedTable.Key.modelcode,
                                     modeldescription = groupedTable.Key.modeldescription,
                                     modellink =groupedTable.Key.modellink,
                                     ModelId = groupedTable.Key.ModelId,
                                     manufactid = groupedTable.Key.manufactid,
                                     ManufacturerCode =groupedTable.Key.ManufacturerCode,
                                     ManufactDesc = groupedTable.Key.ManufactDesc,
                                     ApplianceTypeID = groupedTable.Key.ApplianceTypeID,
                                     ApplianceCode = groupedTable.Key.ApplianceCode,
                                     ApplianceDesc = groupedTable.Key.ApplianceDesc,
                                     StkCategorydescription = groupedTable.Key.StkCategorydescription,
                                     StkCategoryId = groupedTable.Key.StkCategoryId,
                                     StocksearchTermsWebID = groupedTable.Key.StocksearchTermsWebID,
                                     StockSearchentitiesWebPlacing = groupedTable.Key.StockSearchentitiesWebPlacing

                                 };
//select new;
            // DataTable dt2 = UniqueRows;
             // dt2.TableName = "Detailresult";
            DataTable result= ToDataTable(groupQuery);
            result.TableName = "DetailResult";
            return result;

        }
        //private DataTable SearchResult(DataTable dt, string StoredProc, string SAEDIID, string searchText, int entityname)
        //{
        //    SqlParameter[] parameters = { new SqlParameter("@SAEDIID", SAEDIID) ,
        //                                   new SqlParameter("@SearchText", searchText) };
        //    DataTable dtab = (MSSQLAccess.SelectStoredProcedure(StoredProc, parameters)).Tables[0];
        //    if (dtab.Rows.Count > 0)
        //    {
        //        dt = MapDataToTable(dt, dtab, entityname,searchText);
        
        //    }
           
        //    return dt;
        //}

        public  DataSet GetVocabularyList()
        {
            SqlParameter[] parameters = { new SqlParameter("@count", 1000) };
            return MSSQLAccess.SelectStoredProcedure("GetVocabularyList", parameters);
        
        }





        private DataTable MapDataToTable(DataTable dt, DataTable newresult, int entityname, string searchText)
        {
            //   DataTable table = new DataTable();
            foreach (DataRow dr in newresult.Rows)
            {
                DataRow newrow = dt.NewRow();
                newrow["StockId"] = dr["catalogueid"];//StockId"));
                newrow["StkCode"] = dr["StkCode"];//dt.Columns.Add(new DataColumn("StkCode"));
                newrow["StkDescription"] = dr["StkDescription"];//  dt.Columns.Add(new DataColumn("StkDescription"));
                newrow["StockPrice"] = dr["StkSalePrice"];// dt.Columns.Add(new DataColumn("StockPrice"));
                //   newrow["StockSearchEntitiesWebWord"] = searchText;//  dt.Columns.Add(new DataColumn("EntityType"));
                newrow["modelcode"] = (dr.Table.Columns.Contains("modelcode")) ? dr["modelcode"] : string.Empty;// dt.Columns.Add(new DataColumn("modelcode"));
                newrow["modeldescription"] = (dr.Table.Columns.Contains("modeldescription") ? dr["modeldescription"] : string.Empty);// dr["modeldescription"];// dt.Columns.Add(new DataColumn("modeldescription"));
                newrow["modellink"] = (dr.Table.Columns.Contains("modellink") ? dr["modellink"] : string.Empty);
                newrow["ManufacturerCode"] = (dr.Table.Columns.Contains("ManufacturerCode") ? dr["ManufacturerCode"] : string.Empty);
                newrow["ManufactDesc"] = (dr.Table.Columns.Contains("ManufactDesc") ? dr["ManufactDesc"] : string.Empty);
                newrow["ModelId"] = (dr.Table.Columns.Contains("ModelId") ? dr["ModelId"] : string.Empty);
                newrow["StockSearchEntitiesWebScore"] = dr["Score"];
                newrow["StockSearchEntitiesWebEntity"] = entityname;
                newrow["manufactid"] = (dr.Table.Columns.Contains("manufactid") ? dr["manufactid"] : string.Empty);
                newrow["ApplianceTypeID"] = (dr.Table.Columns.Contains("ApplianceTypeID") ? dr["ApplianceTypeID"] : string.Empty);
                newrow["ApplianceCode"] = (dr.Table.Columns.Contains("ApplianceCode") ? dr["ApplianceCode"] : string.Empty);
                newrow["ApplianceDesc"] = (dr.Table.Columns.Contains("ApplianceDesc") ? dr["ApplianceDesc"] : string.Empty);
                newrow["StkCategorydescription"] = dr.Table.Columns.Contains("StkCategorydescription") ? (dr["StkCategorydescription"].ToString()) : string.Empty;
                newrow["StkCategoryId"] = dr.Table.Columns.Contains("StkCategoryId") ? dr["StkCategoryId"] : string.Empty;
                newrow["StockSearchentitiesWebPlacing"] = 0;
                switch (entityname)
                {
                    case (int)StockPartSearchEntity.Stock:
                        searchText = dr["StkCode"].ToString();
                        break;
                    case (int)StockPartSearchEntity.Model:
                        searchText = dr["modelcode"].ToString();
                        break;
                    case (int)StockPartSearchEntity.Appliance:
                        searchText = dr["ApplianceDesc"].ToString();
                        break;
                    case (int)StockPartSearchEntity.Manufacturer:
                        searchText = dr["ManufactDesc"].ToString();
                        break;
                    case (int)StockPartSearchEntity.StockCategory:
                        searchText = dr["StkDescription"].ToString();
                        break;

                }
                newrow["StockSearchEntitiesWebWord"] = searchText;
                dt.Rows.Add(newrow);
            }
            return dt;
        }
       #region IPartsDataProvider Members

        public bool CheckIfCanOrderPartsByClientId(string _client)
        {
            SqlParameter[] parameters = { new SqlParameter("@Client_id", _client) };
            string url;

            try
            {
                DataSet ds = MSSQLAccess.SelectStoredProcedure("fz_getOSPURLByClientID", parameters);
                url = ds.Tables[0].Rows[0]["OSPUrl"].ToString();
                bool usePriceCheck = bool.Parse(ds.Tables[0].Rows[0]["UsePriceCheck"].ToString());
            }
            catch
            {
                url = "";
            }

            List<CallPart> parts = new List<CallPart>();
            if (url != "")
            {
                IOnlineSparePartsservice OSP = new IOnlineSparePartsservice();
                OSP.Url = url;
                string records;
                try
                {
                    records = OSP.QueryOrderProgressEx(_client, "fecker", "");
                    return true;
                }
                catch { }
            }

            return false;
        }

        public List<CallPart> GetPartsByClientId(string _client)
        {
            SqlParameter[] parameters = { new SqlParameter("@Client_id", _client) };
            DataSet ds = MSSQLAccess.SelectStoredProcedure("fz_getOSPURLByClientID", parameters);

            string url = ds.Tables[0].Rows[0]["OSPUrl"].ToString();
            bool usePriceCheck = bool.Parse(ds.Tables[0].Rows[0]["UsePriceCheck"].ToString());

            List<CallPart> parts = new List<CallPart>();
            if (url != "")
            {

                IOnlineSparePartsservice OSP = new IOnlineSparePartsservice();
                OSP.Url = url;


                try
                {
                    string records = OSP.QueryOrderProgressEx(_client, "fecker", "");

                    byte[] byteArray = Encoding.ASCII.GetBytes(records);
                    MemoryStream stream = new MemoryStream(byteArray);

                    XmlTextReader xml = new XmlTextReader(stream);
                    while (xml.Read())
                    {
                        int j = 0;
                        if (xml.AttributeCount != 0)
                        {
                            DateTime dt;
                            CallPart line = new CallPart();
                            try
                            { line.LineNo = int.Parse(xml.GetAttribute("LineNo")); }
                            catch
                            { line.LineNo = -(j + 1); }
                            line.PartReference = int.Parse(xml.GetAttribute("OrderNumber"));
                            line.Code = xml.GetAttribute("StockCode");
                            line.Description = xml.GetAttribute("Description");
                            line.UnitPrice = decimal.Parse(xml.GetAttribute("RetailValue"));
                            line.Quantity = int.Parse(xml.GetAttribute("Quant"));
                            line.TransactionCode = xml.GetAttribute("TransCode");
                            line.Status = xml.GetAttribute("Status");
                            if (DateTime.TryParse(xml.GetAttribute("DispatchDate"), out dt)) line.DispatchDate = dt;
                            if (DateTime.TryParse(xml.GetAttribute("OrderDate"), out dt)) line.OrderDate = dt;
                            line.CourierReference = xml.GetAttribute("CourierRef");
                            line.DeliveryNumber = xml.GetAttribute("DeliveryNo");
                            try { line.OrderReference = xml.GetAttribute("OrderReference") == null ? "" : xml.GetAttribute("OrderReference"); }
                            catch { }
                            try
                            {
                                line.ReturnDescription = xml.GetAttribute("ReturnDescription");
                                line.ReturnReference = xml.GetAttribute("ReturnReference");
                                line.ReturnRequired = bool.Parse(xml.GetAttribute("ReturnRequired"));                                
                            }
                            catch { /* ignore for older web services */}

                            try
                            {
                                line.PartNote = xml.GetAttribute("PartNote");
                            }
                            catch { }
                            line.UsePriceCheck = usePriceCheck;
                            parts.Add(line);
                            j++;
                        }
                    }
                }
                catch
                {
                    return parts;
                }
            }
            return parts;
        }
      
        public List<CallPart> GetPartsByClientIdForSONY(string _callid, string _client, string _serviceID,string PartCode)
        {
        //    OSPRefDataProvider ospDLL = new OSPRefDataProvider();
        //    List<OSPRefs> ospRefsList = ospDLL.GetOSPRefByCallID(_callid);
        //    OSPRefs ospRefs = new OSPRefs();
        //    if (ospRefsList.Count > 0)
        //        ospRefs = ospRefsList[0];
        //    else
        //    {
        //        ospRefs.OSPUrl = string.Empty;
        //        ospRefs.IsUsePriceCheck = false;
        //    }

        //    string url = ospRefs.OSPUrl;
        //    bool usePriceCheck = ospRefs.IsUsePriceCheck;

        //    List<CallPart> parts = new List<CallPart>();
        //    if (url != "")
        //    {
        //        IOnlineSparePartsservice OSP = new IOnlineSparePartsservice();
        //        OSP.Url = url;

        //         //string records = OSP.QueryOrderProgressEx(_client, "fecker", "");

        //         //   byte[] byteArray = Encoding.ASCII.GetBytes(records);
        //         //   MemoryStream stream = new MemoryStream(byteArray);

        //         //   XmlTextReader xml = new XmlTextReader(stream);
        //         //   while (xml.Read())
        //        string records = OSP.QueryOrderProgressEx(_client, "fecker", _serviceID);
              
        //        byte[] byteArray = Encoding.ASCII.GetBytes(records);
        //        MemoryStream stream = new MemoryStream(byteArray);

        //        XmlTextReader xml = new XmlTextReader(stream);
        //        while (xml.Read())
        //        {
        //            int j = 0;
        //            if (xml.AttributeCount != 0)
        //            {
        //                DateTime dt;
        //                CallPart line = new CallPart();
        //                try
        //                { line.LineNo = int.Parse(xml.GetAttribute("LineNo")); }
        //                catch
        //                { line.LineNo = -(j + 1); }
        //                line.PartReference = int.Parse(xml.GetAttribute("OrderNumber"));
        //                line.Code = xml.GetAttribute("StockCode");
        //                line.Description = xml.GetAttribute("Description");
        //                try
        //                {
        //                    line.UnitPrice = decimal.Parse(xml.GetAttribute("RetailValue"));
        //                }
        //                catch
        //                {
        //                    line.UnitPrice = 0;
        //                }
        //                line.Quantity = int.Parse(xml.GetAttribute("Quant"));
        //                line.TransactionCode = xml.GetAttribute("TransCode");
        //                line.Status = xml.GetAttribute("Status");
        //                if (DateTime.TryParse(xml.GetAttribute("DispatchDate"), out dt)) line.DispatchDate = dt;
        //                if (DateTime.TryParse(xml.GetAttribute("OrderDate"), out dt)) line.OrderDate = dt;
        //                line.CourierReference = xml.GetAttribute("CourierRef");
        //                line.DeliveryNumber = xml.GetAttribute("DeliveryNo");
                        
        //                try
        //                {
        //                    line.StatusID = xml.GetAttribute("StatusId");
        //                    line.ReturnDescription = xml.GetAttribute("ReturnDescription");
        //                    line.ReturnReference = xml.GetAttribute("ReturnReference");
        //                    line.ReturnRequired = bool.Parse(xml.GetAttribute("ReturnRequired"));
                           
        //                }
        //                catch { /* ignore for older web services */}
        //                try{
        //                     line.OrderReference = xml.GetAttribute("OrderReference");
        //                }
        //                catch{}
                        
        //                line.UsePriceCheck = usePriceCheck;

        //                try
        //                {
        //                    if (line.StatusID.ToUpper().Trim() == "V")
        //                        parts.Add(line);
        //                }
        //                catch { }

        //                j++;
        //            }
        //        }
        //    }
        //    return parts;
            
      
            CallDataProvider callsDAL = new CallDataProvider();
            Call call = callsDAL.GetById(long.Parse(_callid));

            SqlParameter[] parameters = { new SqlParameter("@_ID", _callid) };
            DataSet ds = MSSQLAccess.SelectStoredProcedure("fz_getOSPURLByCallID", parameters);

            string url = ds.Tables[0].Rows[0][0].ToString();
            bool usePriceCheck = bool.Parse(ds.Tables[0].Rows[0][2].ToString());

            List<CallPart> parts = new List<CallPart>();
            if (url != "")
            {
                IOnlineSparePartsservice OSP = new IOnlineSparePartsservice();
                OSP.Url = url;

                string records = OSP.QueryOrderProgressEx1(call.SaediFromId, "fecker", "", PartCode);
      
                byte[] byteArray = Encoding.ASCII.GetBytes(records);
                MemoryStream stream = new MemoryStream(byteArray);

                XmlTextReader xml = new XmlTextReader(stream);
                while (xml.Read())
                {
                    int j = 0;
                    if (xml.AttributeCount != 0)
                    {
                        DateTime dt;
                        CallPart line = new CallPart();
                        try
                        { line.LineNo = int.Parse(xml.GetAttribute("LineNo")); }
                        catch
                        { line.LineNo = -(j + 1); }

                        line.CourierReference = "";
                        line.DeliveryNumber = "";
                        line.ReturnDescription = "";
                        line.ReturnReference = "";
                        line.ReturnRequired = false;
                        line.OrderReference = "";
                        line.UsePriceCheck = usePriceCheck;
                        try
                        {
                            try { line.PartReference = xml.GetAttribute("OrderNumber") == null ? 0 : int.Parse(xml.GetAttribute("OrderNumber")); }
                            catch { }
                            try { line.Code = xml.GetAttribute("StockCode") == null ? "" : xml.GetAttribute("StockCode"); }
                            catch { }
                            try { line.Description = xml.GetAttribute("Description") == null ? "" : xml.GetAttribute("Description"); }
                            catch { }
                            try { line.UnitPrice = xml.GetAttribute("RetailValue") == null ? 0 : decimal.Parse(xml.GetAttribute("RetailValue")); }
                            catch { }
                            try { line.Quantity = xml.GetAttribute("Quant") == null ? 0 : int.Parse(xml.GetAttribute("Quant")); }
                            catch { }
                            try { line.TransactionCode = xml.GetAttribute("TransCode") == null ? "" : xml.GetAttribute("ReturnDescription"); }
                            catch { }
                            try { line.Status = xml.GetAttribute("Status") == null ? "" : xml.GetAttribute("ReturnDescription"); }
                            catch { }
                            try { line.StatusID = xml.GetAttribute("StatusId"); }
                            catch { line.StatusID = string.Empty; }
                            try { if (DateTime.TryParse(xml.GetAttribute("DispatchDate"), out dt)) line.DispatchDate = dt; }
                            catch { }
                            try { if (DateTime.TryParse(xml.GetAttribute("OrderDate"), out dt)) line.OrderDate = dt; }
                            catch { }
                            try { line.CourierReference = xml.GetAttribute("CourierRef") == null ? "" : xml.GetAttribute("CourierRef"); }
                            catch { }
                            try { line.DeliveryNumber = xml.GetAttribute("DeliveryNo") == null ? "" : xml.GetAttribute("DeliveryNo"); }
                            catch { }
                            try { line.ReturnDescription = xml.GetAttribute("ReturnDescription") == null ? "" : xml.GetAttribute("ReturnDescription"); }
                            catch { }
                            try { line.ReturnReference = xml.GetAttribute("ReturnReference") == null ? "" : xml.GetAttribute("ReturnReference"); }
                            catch { }
                            try { line.ReturnRequired = xml.GetAttribute("ReturnRequired") == null ? false : bool.Parse(xml.GetAttribute("ReturnRequired")); }
                            catch { }
                            try { line.OrderReference = xml.GetAttribute("OrderReference") == null ? "" : xml.GetAttribute("OrderReference"); }
                            catch { }
                            try { line.UsePriceCheck = usePriceCheck; }
                            catch { }
                        }
                        catch { /* ignore for older web services */ }

                        parts.Add(line);
                        j++;
                    }
                }
            }
            return parts;
        }
        // to handle backorder
        public List<CallPart> GetPartsByClientIdForSONYNopartcode(string _callid, string _client, string _serviceID)
        {
            CallDataProvider callsDAL = new CallDataProvider();
            Call call = callsDAL.GetById(long.Parse(_callid));

            SqlParameter[] parameters = { new SqlParameter("@_ID", _callid) };
            DataSet ds = MSSQLAccess.SelectStoredProcedure("fz_getOSPURLByCallID", parameters);

            string url = ds.Tables[0].Rows[0][0].ToString();
            bool usePriceCheck = bool.Parse(ds.Tables[0].Rows[0][2].ToString());

            List<CallPart> parts = new List<CallPart>();
            if (url != "")
            {
                IOnlineSparePartsservice OSP = new IOnlineSparePartsservice();
                OSP.Url = url;

                string records = OSP.QueryOrderProgressEx(call.SaediFromId, "fecker", "");

                byte[] byteArray = Encoding.ASCII.GetBytes(records);
                MemoryStream stream = new MemoryStream(byteArray);

                XmlTextReader xml = new XmlTextReader(stream);
                while (xml.Read())
                {
                    int j = 0;
                    if (xml.AttributeCount != 0)
                    {
                        DateTime dt;
                        CallPart line = new CallPart();
                        try
                        { line.LineNo = int.Parse(xml.GetAttribute("LineNo")); }
                        catch
                        { line.LineNo = -(j + 1); }

                        line.CourierReference = "";
                        line.DeliveryNumber = "";
                        line.ReturnDescription = "";
                        line.ReturnReference = "";
                        line.ReturnRequired = false;
                        line.OrderReference = "";
                        line.UsePriceCheck = usePriceCheck;
                        try
                        {
                            try { line.PartReference = xml.GetAttribute("OrderNumber") == null ? 0 : int.Parse(xml.GetAttribute("OrderNumber")); }
                            catch { }
                            try { line.Code = xml.GetAttribute("StockCode") == null ? "" : xml.GetAttribute("StockCode"); }
                            catch { }
                            try { line.Description = xml.GetAttribute("Description") == null ? "" : xml.GetAttribute("Description"); }
                            catch { }
                            try { line.UnitPrice = xml.GetAttribute("RetailValue") == null ? 0 : decimal.Parse(xml.GetAttribute("RetailValue")); }
                            catch { }
                            try { line.Quantity = xml.GetAttribute("Quant") == null ? 0 : int.Parse(xml.GetAttribute("Quant")); }
                            catch { }
                            try { line.TransactionCode = xml.GetAttribute("TransCode") == null ? "" : xml.GetAttribute("ReturnDescription"); }
                            catch { }
                            try { line.Status = xml.GetAttribute("Status") == null ? "" : xml.GetAttribute("ReturnDescription"); }
                            catch { }
                            try { line.StatusID = xml.GetAttribute("StatusId"); }
                            catch { line.StatusID = string.Empty; }
                            try { if (DateTime.TryParse(xml.GetAttribute("DispatchDate"), out dt)) line.DispatchDate = dt; }
                            catch { }
                            try { if (DateTime.TryParse(xml.GetAttribute("OrderDate"), out dt)) line.OrderDate = dt; }
                            catch { }
                            try { line.CourierReference = xml.GetAttribute("CourierRef") == null ? "" : xml.GetAttribute("CourierRef"); }
                            catch { }
                            try { line.DeliveryNumber = xml.GetAttribute("DeliveryNo") == null ? "" : xml.GetAttribute("DeliveryNo"); }
                            catch { }
                            try { line.ReturnDescription = xml.GetAttribute("ReturnDescription") == null ? "" : xml.GetAttribute("ReturnDescription"); }
                            catch { }
                            try { line.ReturnReference = xml.GetAttribute("ReturnReference") == null ? "" : xml.GetAttribute("ReturnReference"); }
                            catch { }
                            try { line.ReturnRequired = xml.GetAttribute("ReturnRequired") == null ? false : bool.Parse(xml.GetAttribute("ReturnRequired")); }
                            catch { }
                            try { line.OrderReference = xml.GetAttribute("OrderReference") == null ? "" : xml.GetAttribute("OrderReference"); }
                            catch { }
                            try { line.UsePriceCheck = usePriceCheck; }
                            catch { }
                        }
                        catch { /* ignore for older web services */ }

                        parts.Add(line);
                        j++;
                    }
                }
            }
            return parts;
        }
    
        public string GetPartsByClientIdForSONYtest(string _callid, string _client)
        {
            string records = string.Empty;
            try
            {
                CallDataProvider callsDAL = new CallDataProvider();
                Call call = callsDAL.GetById(long.Parse(_callid));

                SqlParameter[] parameters = { new SqlParameter("@_ID", _callid) };
                DataSet ds = MSSQLAccess.SelectStoredProcedure("fz_getOSPURLByCallID", parameters);
                IOnlineSparePartsservice OSP = new IOnlineSparePartsservice();
                string url = ds.Tables[0].Rows[0][0].ToString();
                bool usePriceCheck = bool.Parse(ds.Tables[0].Rows[0][2].ToString());

                List<CallPart> parts = new List<CallPart>();

                records = OSP.QueryOrderProgressEx(call.SaediFromId, "fecker", "");

                byte[] byteArray = Encoding.ASCII.GetBytes(records);
                MemoryStream stream = new MemoryStream(byteArray);
                FileStream file = new FileStream(AppDomain.CurrentDomain.BaseDirectory + @"\file.txt", FileMode.Create, FileAccess.Write);
                stream.WriteTo(file);
                file.Close();

                XmlTextReader xml = new XmlTextReader(stream);

                records += byteArray.Count();

                while (xml.Read())
                {
                    int j = 0;
                    if (xml.AttributeCount != 0)
                    {
                        DateTime dt;
                        CallPart line = new CallPart();
                        try
                        { line.LineNo = int.Parse(xml.GetAttribute("LineNo")); }
                        catch
                        { line.LineNo = -(j + 1); }

                        line.CourierReference = "";
                        line.DeliveryNumber = "";
                        line.ReturnDescription = "";
                        line.ReturnReference = "";
                        line.ReturnRequired = false;
                        line.OrderReference = "";
                        line.UsePriceCheck = usePriceCheck;
                        try
                        {
                            try { line.PartReference = xml.GetAttribute("OrderNumber") == null ? 0 : int.Parse(xml.GetAttribute("OrderNumber")); }
                            catch { }
                            try { line.Code = xml.GetAttribute("StockCode") == null ? "" : xml.GetAttribute("StockCode"); }
                            catch { }
                            try { line.Description = xml.GetAttribute("Description") == null ? "" : xml.GetAttribute("Description"); }
                            catch { }
                            try { line.UnitPrice = xml.GetAttribute("RetailValue") == null ? 0 : decimal.Parse(xml.GetAttribute("RetailValue")); }
                            catch { }
                            try { line.Quantity = xml.GetAttribute("Quant") == null ? 0 : int.Parse(xml.GetAttribute("Quant")); }
                            catch { }
                            try { line.TransactionCode = xml.GetAttribute("TransCode") == null ? "" : xml.GetAttribute("ReturnDescription"); }
                            catch { }
                            try { line.Status = xml.GetAttribute("Status") == null ? "" : xml.GetAttribute("ReturnDescription"); }
                            catch { }
                            try { line.StatusID = xml.GetAttribute("StatusId"); }
                            catch { line.StatusID = string.Empty; }
                            try { if (DateTime.TryParse(xml.GetAttribute("DispatchDate"), out dt)) line.DispatchDate = dt; }
                            catch { }
                            try { if (DateTime.TryParse(xml.GetAttribute("OrderDate"), out dt)) line.OrderDate = dt; }
                            catch { }
                            try { line.CourierReference = xml.GetAttribute("CourierRef") == null ? "" : xml.GetAttribute("CourierRef"); }
                            catch { }
                            try { line.DeliveryNumber = xml.GetAttribute("DeliveryNo") == null ? "" : xml.GetAttribute("DeliveryNo"); }
                            catch { }
                            try { line.ReturnDescription = xml.GetAttribute("ReturnDescription") == null ? "" : xml.GetAttribute("ReturnDescription"); }
                            catch { }
                            try { line.ReturnReference = xml.GetAttribute("ReturnReference") == null ? "" : xml.GetAttribute("ReturnReference"); }
                            catch { }
                            try { line.ReturnRequired = xml.GetAttribute("ReturnRequired") == null ? false : bool.Parse(xml.GetAttribute("ReturnRequired")); }
                            catch { }
                            try { line.OrderReference = xml.GetAttribute("OrderReference") == null ? "" : xml.GetAttribute("OrderReference"); }
                            catch { }
                            try { line.UsePriceCheck = usePriceCheck; }
                            catch { }
                        }
                        catch { /* ignore for older web services */ }

                        parts.Add(line);
                        j++;
                    }
                } stream.Close();
            }
            catch (Exception ex) { records += ex.Message; }
             
                return records;
            
        }

        #endregion




      


        //public DataSet GetPNCRecords(string saedisourceid)
        //{
        //    SqlParameter[] parameters = { new SqlParameter("@saedisourceid", saedisourceid) 
        //                                };
        //    return (MSSQLAccess.SelectStoredProcedure("FZ_GETPNCList ",parameters));
        //}


     public   int   CheckSONUsed(string SON)
        {
             SqlParameter[] parameters = { new SqlParameter("@son", SON) };         
             DataSet ds = MSSQLAccess.SelectStoredProcedure("fz_CheckSONNumber", parameters);
             int ConsumedCount = int.Parse(ds.Tables[0].Rows[0]["ConsumedCount"].ToString());
             return ConsumedCount;
        }


    
    }
}
