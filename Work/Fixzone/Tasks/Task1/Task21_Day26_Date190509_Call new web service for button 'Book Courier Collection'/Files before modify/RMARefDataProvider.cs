using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mobile.Portal.Classes;
using System.Data.SqlClient;
using Mobile.Portal.MSSQL;
using System.Data;
using Mobile.Portal.Helpers;
using System.Xml.Serialization;
// using Mobile.Portal.Services.SonyRMAService;
// using Mobile.Portal.Services.SonyReturnPRD;

namespace Mobile.Portal.DAL
{
    [Serializable]
    public class RMARefDataProvider : DataAccess<RMARef>, Mobile.Portal.DAL.IRMARefDataProvider
    {
        public List<RMARef> RMAGenerateRequest(string saediFromId, string clientRef, string userID, string password, string aepBookInRef, string externalMatId, string claimType, string clientReference,
            string faultCode, string gpToolRmaId, string repairNumber, string modelID, string partNumber, string remark, int returnQuantity, string serialNumber,
            string sonNumber, string partNumberReceived)
              
        {
            Random random = new Random();
            // Call web service REQUEST
            // "https://webservices.staging.vaio.eu/services/AscServiceSync?wsdl"

            RMARef rma = new RMARef();
            SonyServicesHelper ssh = new SonyServicesHelper();
            rma.ClientRef = clientRef;
            rma.SaediFromId = saediFromId;
            rma.INPUT_AepBookinRef = aepBookInRef;
            rma.INPUT_ClaimType = claimType;
            rma.INPUT_clientReference = clientReference;
            rma.INPUT_externalMatId = externalMatId;
            rma.INPUT_faultCode = faultCode;
            rma.INPUT_GpToolRmaId = gpToolRmaId;
            rma.INPUT_modelID = modelID;
            rma.INPUT_PartNumber = partNumber;
            rma.INPUT_partNumberReceived = partNumberReceived;
            rma.INPUT_Remark = remark;
            rma.INPUT_repairNumber = repairNumber;
            rma.INPUT_returnQuantity = returnQuantity.ToString();
            rma.INPUT_serialNumber = serialNumber;
            rma.INPUT_sonNumber = sonNumber;
        
                if (ssh.IsTesting || ssh.IsDevelopment)
                {
                    var request = new Mobile.Portal.Services.SonyRMAService.registerReturnRequest();
                    request.password = password;
                    request.userId = userID;

                    var returnrequest = new Mobile.Portal.Services.SonyRMAService.returnRequest();
                    returnrequest.aepBookingReference = aepBookInRef;
                    returnrequest.ascMaterialId = externalMatId;
                    returnrequest.claimType = claimType;
                    returnrequest.clientReference = clientReference;
                    returnrequest.faultCode = faultCode;
                    returnrequest.gpToolRmaId = gpToolRmaId;
                    returnrequest.mainAscReferenceId = repairNumber; // ????
                    returnrequest.modelName = modelID;
                    returnrequest.mispickedPartNumberReceived = partNumber;
                    returnrequest.remark = remark;
                    returnrequest.returnQty = returnQuantity;
                    returnrequest.serialNumber = serialNumber;
                    returnrequest.son = sonNumber;
                    returnrequest.sonyPartNumber = partNumberReceived; // ????
                    request.returnRequest = returnrequest;

                    var r = new Mobile.Portal.Services.SonyRMAService.ReturnRegistrationWebServiceClient("ReturnRegistrationWebServiceImplPort");
                    Mobile.Portal.Services.SonyRMAService.registerReturnWsResponse response = r.registerReturn(request);

                    const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                    var test = new string(Enumerable.Repeat(chars, 6)
                      .Select(s => s[random.Next(s.Length)]).ToArray());

                    //rma.success = true;// response.success;
                    //rma.errorMessage = "";// response.errorMessage;
                    //rma.rmaDocumentUrl = @"https://ibiss.crse.com/cse-return-module/documents/attachment/b816dc7a-9d86-45a7-80ee-47b550543b45/rmaDocument.pdf";// response.rmaDocumentUrl;
                    //rma.rmaId = test;// response.rmaId;
                    //rma.shipmentStatus = "";// response.shipmentStatus;
                    //rma.validationErrorList = response.validationErrorList;
                    //rma.validationStatus = "";// response.validationStatus;
                    rma.rmaDocumentUrl = response.rmaDocumentUrl;
                    rma.rmaId = response.rmaId;
                    rma.shipmentStatus = response.shipmentStatus;
                    rma.validationErrorList = response.validationErrorList;
                    rma.validationStatus = response.validationStatus;
                    rma.success = response.success;
                    rma.errorMessage = response.errorMessage;
                }
                else
                {
                    var request = new Mobile.Portal.Services.SonyReturnPRD.registerReturnRequest();
                    request.password = password;
                    request.userId = userID;

                    var returnrequest = new Mobile.Portal.Services.SonyReturnPRD.returnRequest();
                    returnrequest.aepBookingReference = aepBookInRef;
                    returnrequest.ascMaterialId = externalMatId;
                    returnrequest.claimType = claimType;
                    returnrequest.clientReference = clientReference;
                    returnrequest.faultCode = faultCode;
                    returnrequest.gpToolRmaId = gpToolRmaId;
                    returnrequest.mainAscReferenceId = repairNumber; // ????
                    returnrequest.modelName = modelID;
                    returnrequest.mispickedPartNumberReceived = partNumber;
                    returnrequest.remark = remark;
                    returnrequest.returnQty = returnQuantity;
                    returnrequest.serialNumber = serialNumber;
                    returnrequest.son = sonNumber;
                    returnrequest.sonyPartNumber = partNumberReceived; // ????
                    request.returnRequest = returnrequest;

                  try{  var r = new Mobile.Portal.Services.SonyReturnPRD.ReturnRegistrationWebServiceClient("ReturnRegistrationWebServiceImplPort");
                    Mobile.Portal.Services.SonyReturnPRD.registerReturnWsResponse 
                      response = r.registerReturn(request);
                    //    ErrorHandler.LogToFile(session, request);
                
           
                    rma.success = response.success;
                    rma.errorMessage = response.errorMessage ;
                    rma.rmaDocumentUrl = response.rmaDocumentUrl;
                    rma.rmaId = response.rmaId;
                    rma.shipmentStatus = response.shipmentStatus;
                    rma.validationErrorList = response.validationErrorList;
                    rma.validationStatus = response.validationStatus;
                   } catch (Exception ex)
            { rma.errorMessage=rma.errorMessage+ "--Input :" + ToXML(request.returnRequest) +"...exception: .."+ex.Message;
            }
                }

           
            List<RMARef> rmaList = new List<RMARef>();
            rmaList.Add(rma);

            return rmaList;                    
        }
        public string ToXML(object o)
        {
            var stringwriter = new System.IO.StringWriter();
            var serializer = new XmlSerializer(o.GetType());
            serializer.Serialize(stringwriter, o);
            return stringwriter.ToString();
        }
        public void InsertRMA(RMARef rma)
        {
            int rowsAffected;

            string errorList = string.Empty;
            try
            {
                foreach (string er in rma.validationErrorList)
                {
                    errorList += er + "< /br>";
                }
            }
            catch {}

            SqlParameter[] parameters = 
                        {		
                            // OUTPUT:
                            new SqlParameter("@SAEDIFromID", rma.SaediFromId),
                            new SqlParameter("@ClientRef", rma.ClientRef),
                            new SqlParameter("@Success", rma.success == true ? 1 : 0),
                            new SqlParameter("@ErrorMessage", rma.errorMessage == null ? string.Empty : rma.errorMessage),
                            new SqlParameter("@RmaDocumentUrl", rma.rmaDocumentUrl == null ? string.Empty : rma.rmaDocumentUrl),
                            new SqlParameter("@RmaId", rma.rmaId == null ? string.Empty : rma.rmaId),
                            new SqlParameter("@ShipmentStatus", rma.shipmentStatus == null ? string.Empty : rma.shipmentStatus),
                            new SqlParameter("@ValidationErrorList", errorList),
                            new SqlParameter("@ValidationStatus", rma.validationStatus == null ? string.Empty : rma.validationStatus),

                            // INPUT:
                            new SqlParameter("@INPUT_AepBookinRef", rma.INPUT_AepBookinRef),
                            new SqlParameter("@INPUT_ClaimType", rma.INPUT_ClaimType),
                            new SqlParameter("@INPUT_clientReference", rma.INPUT_clientReference),
                            new SqlParameter("@INPUT_externalMatId", rma.INPUT_externalMatId),
                            new SqlParameter("@INPUT_faultCode", rma.INPUT_faultCode),
                            new SqlParameter("@INPUT_GpToolRmaId", rma.INPUT_GpToolRmaId),
                            new SqlParameter("@INPUT_modelID", rma.INPUT_modelID),
                            new SqlParameter("@INPUT_PartNumber", rma.INPUT_PartNumber),
                            new SqlParameter("@INPUT_partNumberReceived", rma.INPUT_partNumberReceived),
                            new SqlParameter("@INPUT_Remark", rma.INPUT_Remark),
                            new SqlParameter("@INPUT_repairNumber", rma.INPUT_repairNumber),
                            new SqlParameter("@INPUT_returnQuantity", rma.INPUT_returnQuantity),
                            new SqlParameter("@INPUT_serialNumber", rma.INPUT_serialNumber),
                            new SqlParameter("@INPUT_sonNumber", rma.INPUT_sonNumber)
                        };
            ExecuteStoredProc("fz_createRMA", parameters, out rowsAffected);
        }

        public void UpdateCollectionjob(string RMA, string jobref, string collectionDate, bool SwapclaimCollection)
        {
            int rowsAffected;
            
            SqlParameter[] parameters = 
                        {		
                            // OUTPUT:
                            new SqlParameter("@RMA", RMA),
                            new SqlParameter("@jobref",jobref),
                                 new SqlParameter("@collectionDate",collectionDate),
                                             new SqlParameter("@SwapclaimCollection",SwapclaimCollection)
                        };
            ExecuteStoredProc("UpdateCollectionjob", parameters, out rowsAffected);
        }
          public List<RMARef> GetPartsRMADetails(string SON, String SaediId)
        {
           // List<RMARef> RMAdetails = new List<RMARef>();
            SqlParameter[] parameters = { new SqlParameter("@SON", SON) ,
                                          
                                           new SqlParameter("@SAEDIFromID", SaediId) };
            return SelectStoredProc("fz_GetPartsRMADetails", parameters);


        }

          
          public RMARef GetPartsRMADetailsbyRMAid(String RMAid, string SAEDIFromID)
          {
              // List<RMARef> RMAdetails = new List<RMARef>();
              SqlParameter[] parameters = { new SqlParameter("@RMAid", RMAid), new SqlParameter("@SAEDIFromID", SAEDIFromID) };
              return SelectStoredProc("fz_GetPartsRMADetailsbyRMAid", parameters).First();


          }
          public List<RMARef> GetPartsRMADetailsbyCall(String Clientref, string SAEDIFromID)
          {
              // List<RMARef> RMAdetails = new List<RMARef>();
              SqlParameter[] parameters = { new SqlParameter("@Clientref", Clientref), new SqlParameter("@SAEDIFromID", SAEDIFromID) };
              return SelectStoredProc("fz_GetPartsRMADetailsbyCall", parameters);


          }
        public override RMARef MapDataToClass(System.Data.DataRow row)
        {
            RMARef rma = new RMARef();

            rma.rmaId = row["rmaId"].ToString();
            rma.shipmentStatus = row["shipmentStatus"].ToString();
            rma.rmaDocumentUrl = row["rmaDocumentUrl"].ToString();
            rma.CollectionDate = ( string.IsNullOrEmpty(row["CollectionDate"].ToString() )) ? "" : DateTime.Parse(row["CollectionDate"].ToString()).ToString("dd/MM/yyyy");
            rma.CollectionAddedOn = row["CollectionAddedOn"].ToString();
            rma.Collectionref = row["Collectionref"].ToString();
            rma.ClientRef = row["ClientRef"]==null?"":row["ClientRef"].ToString();
            rma.INPUT_sonNumber = row["INPUT_sonNumber"] == null ? "" : row["INPUT_sonNumber"].ToString();
            rma.INPUT_PartNumber = row["INPUT_PartNumber"]==null?"": row["INPUT_PartNumber"].ToString();
            try
            {
                rma.Partdesc = row["Partdesc"] == null ? "" : row["Partdesc"].ToString();
            }
            catch (Exception ex)
            {
                rma.Partdesc = "";
            }
            
            return rma;
        }


        
    }
}
