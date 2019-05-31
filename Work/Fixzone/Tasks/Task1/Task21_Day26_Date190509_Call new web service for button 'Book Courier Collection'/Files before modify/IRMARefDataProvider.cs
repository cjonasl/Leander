using Mobile.Portal.Classes;
using System;
using System.Collections.Generic;
namespace Mobile.Portal.DAL
{
    public interface IRMARefDataProvider
    {
        System.Collections.Generic.List<Mobile.Portal.Classes.RMARef> RMAGenerateRequest(string saediFromId, string clientRef, string userID, string password, string aepBookInRef, string externalMatId, string claimType, string clientReference,
            string faultCode, string gpToolRmaId, string repairNumber, string modelID, string partNumber, string remark, int returnQuantity, string serialNumber,
            string sonNumber, string partNumberReceived);
        void InsertRMA(Mobile.Portal.Classes.RMARef rma);
        List<RMARef> GetPartsRMADetails(string OrderReference, string SAEDIFromID);
        List<RMARef> GetPartsRMADetailsbyCall(String Clientref, string SAEDIFromID);

        void UpdateCollectionjob(string RMA, string jobref, string collectionDate, bool SwapclaimCollection);

        RMARef GetPartsRMADetailsbyRMAid(String RMAid, string SAEDIFromID);
    }
}
