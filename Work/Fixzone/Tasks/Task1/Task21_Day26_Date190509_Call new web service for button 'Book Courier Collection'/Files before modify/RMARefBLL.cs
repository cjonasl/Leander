using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mobile.Portal.Classes;
using Mobile.Portal.DAL;

namespace Mobile.Portal.BLL
{
    public class RMARefBLL : BaseBLL<RMARef>, Mobile.Portal.BLL.IRMARefBLL
    {
        IRMARefDataProvider _dal;

        public RMARefBLL()
        {
            _dal = new RMARefDataProvider();
        }

        public List<RMARef> RMAGenerateRequest(string saediFromId, string clientRef, string userID, string password, string aepBookInRef, string externalMatId, string claimType, string clientReference,
            string faultCode, string gpToolRmaId, string repairNumber, string modelID, string partNumber, string remark, int returnQuantity, string serialNumber,
            string sonNumber, string partNumberReceived)
        {
            return _dal.RMAGenerateRequest(saediFromId, clientRef,userID,  password,  aepBookInRef,  externalMatId,  claimType,  clientReference,
             faultCode,  gpToolRmaId,  repairNumber,  modelID,  partNumber,  remark,  returnQuantity,  serialNumber,
             sonNumber,  partNumberReceived);
        }

        public void InsertRMA(RMARef rma)
        {
            _dal.InsertRMA(rma);
        }
        public List<RMARef> GetPartsRMADetailsbyCall(String Clientref, string SAEDIFromID)
        {
           return _dal.GetPartsRMADetailsbyCall(Clientref,SAEDIFromID) ;
        }

        public void UpdateCollectionjob(string RMA, string jobref, string collectionDate, bool SwapclaimCollection)
        {
            _dal.UpdateCollectionjob(RMA, jobref, collectionDate, SwapclaimCollection);
        }
        public RMARef.RmaResponseForView FormatRMAResponse(RMARef response)
        {
            RMARef.RmaResponseForView view = new RMARef.RmaResponseForView();
            if (response == null)
                return view;

            try
            {
                view.StockCode = (response.INPUT_PartNumber == string.Empty ? "N/A" : response.INPUT_PartNumber);
                view.IsError = !response.success;
                view.ErrorMessage = string.Empty;
                view.Success = (response.success == true ? "RMA succesfully generated" : "<span style='color:red;'>ERROR</span>");

                try
                {
                    if (response.errorMessage != string.Empty || response.errorMessage != null)
                    {
                        if (response.errorMessage.ToLower().Contains("claimable quantity exceeded") &&  // For this error show additional message
                            response.statusID == "V")                                                   // Only for stock parts
                            response.errorMessage += "<br/><br/>" +
                                "This error indicates that the SON number used on this claim has been used on a previous claim. " +
                                "Sony keep track of SON numbers and match this to the parts claimed. Once a SON number has been used " +
                                "to claim the number of parts on the original order this error will be given by Sony. You will need to " +
                                "remove this stock part and use an unclaimed SON number.";

                        view.ErrorMessage = "Registering Part RMA<br/>" + response.errorMessage;
                        view.ErrorList = string.Empty;
                        try
                        {
                            if (response.validationErrorList.Count() > 0)
                            {
                                foreach (string er in response.validationErrorList)
                                {
                                    view.ErrorList += "<br />" + er.ToString();
                                }
                            }
                        }
                        catch { }
                    }
                }
                catch {}
                                
                view.Rma = response.rmaId;
                view.ValidationStatus = response.validationStatus;
                view.ShipmentStatus = response.shipmentStatus;
                view.DocumentURL = response.rmaDocumentUrl ==
                                string.Empty ? "Part does not require return." : "<a href='" + response.rmaDocumentUrl + "'> " + response.rmaDocumentUrl + " </a>";
            }
            catch(Exception exc) {
                view = new RMARef.RmaResponseForView();
                view.IsError = true;
                view.Success = "<span style='color:red;'>ERROR</span>";
                view.ErrorMessage = "RMA RESPONSE ERROR! " + exc.Message;
            }

            return view;
        }

        public RMARef GetPartsRMADetailsbyRMAid(string RMAid, string SAEDIID)
        {
            return _dal.GetPartsRMADetailsbyRMAid(RMAid, SAEDIID);
        }
    }
}
