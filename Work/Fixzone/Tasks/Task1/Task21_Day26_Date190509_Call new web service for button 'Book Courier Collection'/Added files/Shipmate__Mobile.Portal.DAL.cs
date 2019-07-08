using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mobile.Portal.Classes;
using System.Data.SqlClient;
using Mobile.Portal.MSSQL;
using System.Data;

namespace Mobile.Portal.DAL
{
    public interface IShipmateDataProvider
    {
        int CreateLogEntry(ShipmateConsignmentDetails data, bool addResponseParameters);
        ShipmateConsignmentDetails GetShipmateConsignmentDetails(string trackingReference);
    }

    public interface IShipmateConfigProvider
    {
        string ShipmateConfig(string clientId = null, string config = null, string trackingReference = null);
    }

    /// <summary>
    /// Req = Was sent in request json-string
    /// Res = In response json-string
    /// </summary>
    public class ShipmateConsignmentDetails
    {
        public string ClientId { get; set; }
        public bool SendRequestSuccess { get; set; }
        public string SendRequestErrorMessage { get; set; }
        public string ResTrackingReference { get; set; }
        public DateTime? LabelCreated { get; set; }
        public DateTime? Manifested { get; set; }
        public DateTime? Collected { get; set; }
        public DateTime? InTransit { get; set; }
        public DateTime? Delivered { get; set; }
        public DateTime? DeliveryFailed { get; set; }
        public int ReqServiceID { get; set; }
        public int ReqRemittanceID { get; set; }
        public string ReqConsignmentReference { get; set; }
        public string ReqServiceKey { get; set; }
        public string ReqCollectionFromName { get; set; }
        public string ReqCollectionFromLine1 { get; set; }
        public string ReqCollectionFromLine2 { get; set; }
        public string ReqCollectionFromLine3 { get; set; }
        public string ReqCollectionFromCompanyName { get; set; }
        public string ReqCollectionFromTelephone { get; set; }
        public string ReqCollectionFromEmailAddress { get; set; }
        public string ReqCollectionFromCity { get; set; }
        public string ReqCollectionFromPostcode { get; set; }
        public string ReqCollectionFromCountry { get; set; }
        public string ReqDeliveryToName { get; set; }
        public string ReqDeliveryToLine1 { get; set; }
        public string ReqDeliveryToLine2 { get; set; }
        public string ReqDeliveryToLine3 { get; set; }
        public string ReqDeliveryToCompanyName { get; set; }
        public string ReqDeliveryToTelephone { get; set; }
        public string ReqDeliveryToEmailAddress { get; set; }
        public string ReqDeliveryToCity { get; set; }
        public string ReqDeliveryToPostcode { get; set; }
        public string ReqDeliveryToCountry { get; set; }
        public string ReqParcelReference { get; set; }
        public int ReqParcelWeight { get; set; }
        public int ReqParcelWidth { get; set; }
        public int ReqParcelLength { get; set; }
        public int ReqParcelDepth { get; set; }
        public string ResMessage { get; set; }
        public string ResConsignmentReference { get; set; }
        public string ResParcelReference { get; set; }
        public string ResCarrier { get; set; }
        public string ResServiceName { get; set; }
        public string ResCreatedBy { get; set; }
        public string ResCreatedWith { get; set; }
        public DateTime? ResCreatedAt { get; set; }
        public string ResMediaURL { get; set; }
        public string ResMediaGUID { get; set; }
    }

    public class ShipmateDataProvider : DataAccess<ShipmateConsignmentDetails>, IShipmateDataProvider
    {
        public int CreateLogEntry(ShipmateConsignmentDetails data, bool addResponseParameters)
        {
            int rowsAffected, n = -1;
            SqlParameter[] parameters;

            if (addResponseParameters)
                parameters = new SqlParameter[44];
            else
                parameters = new SqlParameter[34];

            parameters[++n] = new SqlParameter("@ClientId", data.ClientId);
            parameters[++n] = new SqlParameter("@SendRequestSuccess", data.SendRequestSuccess);

            if (data.SendRequestErrorMessage == null)
                parameters[++n] = new SqlParameter("@SendRequestErrorMessage", DBNull.Value);
            else
                parameters[++n] = new SqlParameter("@SendRequestErrorMessage", data.SendRequestErrorMessage);

            if (data.ResTrackingReference == null)
                parameters[++n] = new SqlParameter("@ResTrackingReference", DBNull.Value);
            else
                parameters[++n] = new SqlParameter("@ResTrackingReference", data.ResTrackingReference);

            if (data.LabelCreated == null)
                parameters[++n] = new SqlParameter("@LabelCreated", DBNull.Value);
            else
                parameters[++n] = new SqlParameter("@LabelCreated", data.LabelCreated.Value.ToString("yyyy-MM-dd HH:mm:ss"));

            parameters[++n] = new SqlParameter("@ReqServiceID", data.ReqServiceID);
            parameters[++n] = new SqlParameter("@ReqRemittanceID", data.ReqRemittanceID);
            parameters[++n] = new SqlParameter("@ReqConsignmentReference", data.ReqConsignmentReference);
            parameters[++n] = new SqlParameter("@ReqServiceKey", data.ReqServiceKey);
            parameters[++n] = new SqlParameter("@ReqCollectionFromName", data.ReqCollectionFromName);
            parameters[++n] = new SqlParameter("@ReqCollectionFromLine1", data.ReqCollectionFromLine1);
            parameters[++n] = new SqlParameter("@ReqCollectionFromLine2", data.ReqCollectionFromLine2);
            parameters[++n] = new SqlParameter("@ReqCollectionFromLine3", data.ReqCollectionFromLine3);
            parameters[++n] = new SqlParameter("@ReqCollectionFromCompanyName", data.ReqCollectionFromCompanyName);
            parameters[++n] = new SqlParameter("@ReqCollectionFromTelephone", data.ReqCollectionFromTelephone);
            parameters[++n] = new SqlParameter("@ReqCollectionFromEmailAddress", data.ReqDeliveryToEmailAddress);
            parameters[++n] = new SqlParameter("@ReqCollectionFromCity", data.ReqCollectionFromCity);
            parameters[++n] = new SqlParameter("@ReqCollectionFromPostcode", data.ReqCollectionFromPostcode);
            parameters[++n] = new SqlParameter("@ReqCollectionFromCountry", data.ReqCollectionFromCountry);
            parameters[++n] = new SqlParameter("@ReqDeliveryToName", data.ReqDeliveryToName);
            parameters[++n] = new SqlParameter("@ReqDeliveryToLine1", data.ReqDeliveryToLine1);
            parameters[++n] = new SqlParameter("@ReqDeliveryToLine2", data.ReqDeliveryToLine2);
            parameters[++n] = new SqlParameter("@ReqDeliveryToLine3", data.ReqDeliveryToLine3);
            parameters[++n] = new SqlParameter("@ReqDeliveryToCompanyName", data.ReqDeliveryToCompanyName);
            parameters[++n] = new SqlParameter("@ReqDeliveryToTelephone", data.ReqDeliveryToTelephone);
            parameters[++n] = new SqlParameter("@ReqDeliveryToEmailAddress", data.ReqDeliveryToEmailAddress);
            parameters[++n] = new SqlParameter("@ReqDeliveryToCity", data.ReqDeliveryToCity);
            parameters[++n] = new SqlParameter("@ReqDeliveryToPostcode", data.ReqDeliveryToPostcode);
            parameters[++n] = new SqlParameter("@ReqDeliveryToCountry", data.ReqDeliveryToCountry);
            parameters[++n] = new SqlParameter("@ReqParcelReference", data.ReqParcelReference);
            parameters[++n] = new SqlParameter("@ReqParcelWeight", data.ReqParcelWeight);
            parameters[++n] = new SqlParameter("@ReqParcelWidth", data.ReqParcelWidth);
            parameters[++n] = new SqlParameter("@ReqParcelLength", data.ReqParcelLength);
            parameters[++n] = new SqlParameter("@ReqParcelDepth", data.ReqParcelDepth);

            if (addResponseParameters)
            {
                parameters[++n] = new SqlParameter("@ResMessage", data.ResMessage);
                parameters[++n] = new SqlParameter("@ResConsignmentReference", data.ResConsignmentReference);
                parameters[++n] = new SqlParameter("@ResParcelReference", data.ResParcelReference);
                parameters[++n] = new SqlParameter("@ResCarrier", data.ResCarrier);
                parameters[++n] = new SqlParameter("@ResServiceName", data.ResServiceName);
                parameters[++n] = new SqlParameter("@ResCreatedBy", data.ResCreatedBy);
                parameters[++n] = new SqlParameter("@ResCreatedWith", data.ResCreatedWith);
                parameters[++n] = new SqlParameter("@ResCreatedAt", data.ResCreatedAt.Value.ToString("yyyy-MM-dd HH:mm:ss"));
                parameters[++n] = new SqlParameter("@ResMediaURL", data.ResMediaURL);
                parameters[++n] = new SqlParameter("@ResMediaGUID", data.ResMediaGUID);
            }

            return ExecuteStoredProc("fz_LogShipmateConsignmentRequestResponse", parameters, out rowsAffected);
        }

        public ShipmateConsignmentDetails GetShipmateConsignmentDetails(string trackingReference)
        {
            SqlParameter[] parameter = new SqlParameter[] { new SqlParameter("@TrackingReference", trackingReference) };
            List<ShipmateConsignmentDetails> list = SelectStoredProc("fz_GetShipmateConsignmentDetails", parameter);
            return list[0];
        }

        public override ShipmateConsignmentDetails MapDataToClass(System.Data.DataRow row)
        {
            ShipmateConsignmentDetails scd = new ShipmateConsignmentDetails();

            scd.ResTrackingReference = row["ResTrackingReference"].ToString();
            scd.LabelCreated = row["LABEL_CREATED"] == DBNull.Value ? null : (DateTime?)DateTime.Parse(row["LABEL_CREATED"].ToString());
            scd.Manifested = row["MANIFESTED"] == DBNull.Value ? null : (DateTime?)DateTime.Parse(row["MANIFESTED"].ToString());
            scd.Collected = row["COLLECTED"] == DBNull.Value ? null : (DateTime?)DateTime.Parse(row["COLLECTED"].ToString());
            scd.InTransit = row["IN_TRANSIT"] == DBNull.Value ? null : (DateTime?)DateTime.Parse(row["IN_TRANSIT"].ToString());
            scd.Delivered = row["DELIVERED"] == DBNull.Value ? null : (DateTime?)DateTime.Parse(row["DELIVERED"].ToString());
            scd.DeliveryFailed = row["DELIVERY_FAILED"] == DBNull.Value ? null : (DateTime?)DateTime.Parse(row["DELIVERY_FAILED"].ToString());
            scd.ReqServiceID = int.Parse(row["ReqServiceID"].ToString());
            scd.ReqRemittanceID = int.Parse(row["ReqRemittanceID"].ToString());
            scd.ReqConsignmentReference = row["ReqConsignmentReference"].ToString();
            scd.ReqServiceKey = row["ReqServiceKey"].ToString();
            scd.ReqCollectionFromName = row["ReqCollectionFromName"].ToString();
            scd.ReqCollectionFromLine1 = row["ReqCollectionFromLine1"].ToString();
            scd.ReqCollectionFromLine2 = row["ReqCollectionFromLine2"].ToString();
            scd.ReqCollectionFromLine3 = row["ReqCollectionFromLine3"].ToString();
            scd.ReqCollectionFromCompanyName = row["ReqCollectionFromCompanyName"].ToString();
            scd.ReqCollectionFromTelephone = row["ReqCollectionFromTelephone"].ToString();
            scd.ReqCollectionFromEmailAddress = row["ReqCollectionFromEmailAddress"].ToString();
            scd.ReqCollectionFromCity = row["ReqCollectionFromCity"].ToString();
            scd.ReqCollectionFromPostcode = row["ReqCollectionFromPostcode"].ToString();
            scd.ReqCollectionFromCountry = row["ReqCollectionFromCountry"].ToString();
            scd.ReqDeliveryToName = row["ReqDeliveryToName"].ToString();
            scd.ReqDeliveryToLine1 = row["ReqDeliveryToLine1"].ToString();
            scd.ReqDeliveryToLine2 = row["ReqDeliveryToLine2"].ToString();
            scd.ReqDeliveryToLine3 = row["ReqDeliveryToLine3"].ToString();
            scd.ReqDeliveryToCompanyName = row["ReqDeliveryToCompanyName"].ToString();
            scd.ReqDeliveryToTelephone = row["ReqDeliveryToTelephone"].ToString();
            scd.ReqDeliveryToEmailAddress = row["ReqDeliveryToEmailAddress"].ToString();
            scd.ReqDeliveryToCity = row["ReqDeliveryToCity"].ToString();
            scd.ReqDeliveryToPostcode = row["ReqDeliveryToPostcode"].ToString();
            scd.ReqDeliveryToCountry = row["ReqDeliveryToCountry"].ToString();
            scd.ReqParcelReference = row["ReqParcelReference"].ToString();
            scd.ReqParcelWeight = int.Parse(row["ReqParcelWeight"].ToString());
            scd.ReqParcelWidth = int.Parse(row["ReqParcelWidth"].ToString());
            scd.ReqParcelLength = int.Parse(row["ReqParcelLength"].ToString());
            scd.ReqParcelDepth = int.Parse(row["ReqParcelDepth"].ToString());
            scd.ResConsignmentReference = row["ResConsignmentReference"].ToString();
            scd.ResParcelReference = row["ResParcelReference"].ToString();
            scd.ResCarrier = row["ResCarrier"].ToString();
            scd.ResServiceName = row["ResServiceName"].ToString();
            scd.ResCreatedBy = row["ResCreatedBy"].ToString();
            scd.ResCreatedWith = row["ResCreatedWith"].ToString();
            scd.ResCreatedAt = row["ResCreatedAt"] == DBNull.Value ? null : (DateTime?)DateTime.Parse(row["ResCreatedAt"].ToString());
            scd.ResMediaGUID = row["ResMediaGUID"].ToString();

            return scd;
        }
    }

    public class ShipmateConfigProvider : DataAccess<string>, IShipmateConfigProvider
    {
        public string ShipmateConfig(string clientId = null, string config = null, string trackingReference = null)
        {
            SqlParameter[] parameters;

            if (!string.IsNullOrEmpty(clientId) && !string.IsNullOrEmpty(config))
            {
                parameters = new SqlParameter[2];

                parameters[0] = new SqlParameter("@SAEDIID", clientId);
                parameters[1] = new SqlParameter("@ShipmateConfig", config);
            }
            else
            {
                parameters = new SqlParameter[1];

                if (!string.IsNullOrEmpty(clientId))
                    parameters[0] = new SqlParameter("@SAEDIID", clientId);
                else
                    parameters[0] = new SqlParameter("@ResTrackingReference", trackingReference);
            }

            List<string> list = SelectStoredProc("fz_ShipmateConfig", parameters);

            return list[0];
        }

        public override string MapDataToClass(System.Data.DataRow row)
        {
            return row[0].ToString();
        }
    }
}