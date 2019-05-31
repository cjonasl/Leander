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
        int CreateLogEntry(ShipmateConsignmentRequestResponse s, bool addResponseParameters);
    }
    
    public class ShipmateConsignmentRequestResponse
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public int ReqServiceID { get; set; }
        public int ReqRemittanceID { get; set; }
        public string ReqConsignmentReference { get; set; }
        public string ReqServiceKey { get; set; }
        public string ReqName { get; set; }
        public string ReqLine1 { get; set; }
        public string ReqCity { get; set; }
        public string ReqPostcode { get; set; }
        public string ReqCountry { get; set; }
        public string ReqReference { get; set; }
        public int ReqWeight { get; set; }
        public int ReqWidth { get; set; }
        public int ReqLength { get; set; }
        public int ReqDepth { get; set; }
        public string RepMessage { get; set; }
        public string RepConsignmentReference { get; set; }
        public string RepParcelReference { get; set; }
        public string RepCarrier { get; set; }
        public string RepServiceName { get; set; }
        public string RepTrackingReference { get; set; }
        public string RepCreatedBy { get; set; }
        public string RepCreatedWith { get; set; }
        public DateTime RepCreatedAt { get; set; }
        public string RepDeliveryName { get; set; }
        public string RepLine1 { get; set; }
        public string RepLine2 { get; set; }
        public string RepLine3 { get; set; }
        public string RepCity { get; set; }
        public string RepCounty { get; set; }
        public string RepPostcode { get; set; }
        public string RepCountry { get; set; }
        public string RepPdf { get; set; }
        public string RepZpl { get; set; }
        public string RepPng { get; set; }
    }

    public class ShipmateDataProvider : DataAccess<ShipmateConsignmentRequestResponse>, IShipmateDataProvider
    {
        public int CreateLogEntry(ShipmateConsignmentRequestResponse s, bool addResponseParameters)
        {
            int rowsAffected;
            SqlParameter[] parameters;

            if (addResponseParameters)

                parameters = new SqlParameter[36];
            else
                parameters = new SqlParameter[16];

            parameters[0] = new SqlParameter("@Success", s.Success);

            if (s.ErrorMessage == null)
                parameters[1] = new SqlParameter("@ErrorMessage", DBNull.Value);
            else
                parameters[1] = new SqlParameter("@ErrorMessage", s.ErrorMessage);

            parameters[2] = new SqlParameter("@ReqServiceID", s.ReqServiceID);
            parameters[3] = new SqlParameter("@ReqRemittanceID", s.ReqRemittanceID);
            parameters[4] = new SqlParameter("@ReqConsignmentReference", s.ReqConsignmentReference);
            parameters[5] = new SqlParameter("@ReqServiceKey", s.ReqServiceKey);
            parameters[6] = new SqlParameter("@ReqName", s.ReqName);
            parameters[7] = new SqlParameter("@ReqLine1", s.ReqLine1);
            parameters[8] = new SqlParameter("@ReqCity", s.ReqCity);
            parameters[9] = new SqlParameter("@ReqPostcode", s.ReqPostcode);
            parameters[10] = new SqlParameter("@ReqCountry", s.ReqCountry);
            parameters[11] = new SqlParameter("@ReqReference", s.ReqReference);
            parameters[12] = new SqlParameter("@ReqWeight", s.ReqWeight);
            parameters[13] = new SqlParameter("@ReqWidth", s.ReqWidth);
            parameters[14] = new SqlParameter("@ReqLength", s.ReqLength);
            parameters[15] = new SqlParameter("@ReqDepth", s.ReqDepth);

            if (addResponseParameters)
            {
                parameters[16] = new SqlParameter("@RepMessage", s.RepMessage);
                parameters[17] = new SqlParameter("@RepConsignmentReference", s.RepConsignmentReference);
                parameters[18] = new SqlParameter("@RepParcelReference", s.RepParcelReference);
                parameters[19] = new SqlParameter("@RepCarrier", s.RepCarrier);
                parameters[20] = new SqlParameter("@RepServiceName", s.RepServiceName);
                parameters[21] = new SqlParameter("@RepTrackingReference", s.RepTrackingReference);
                parameters[22] = new SqlParameter("@RepCreatedBy", s.RepCreatedBy);
                parameters[23] = new SqlParameter("@RepCreatedWith", s.RepCreatedWith);
                parameters[24] = new SqlParameter("@RepCreatedAt", s.RepCreatedAt);
                parameters[25] = new SqlParameter("@RepDeliveryName", s.RepDeliveryName);
                parameters[26] = new SqlParameter("@RepLine1", s.RepLine1);
                parameters[27] = new SqlParameter("@RepLine2", s.RepLine2);
                parameters[28] = new SqlParameter("@RepLine3", s.RepLine3);
                parameters[29] = new SqlParameter("@RepCity", s.RepCity);
                parameters[30] = new SqlParameter("@RepCounty", s.RepCounty);
                parameters[31] = new SqlParameter("@RepPostcode", s.RepPostcode);
                parameters[32] = new SqlParameter("@RepCountry", s.RepCountry);
                parameters[33] = new SqlParameter("@RepPdf", s.RepPdf);
                parameters[34] = new SqlParameter("@RepZpl", s.RepZpl);
                parameters[35] = new SqlParameter("@RepPng", s.RepPng);
            }

            return ExecuteStoredProc("fz_LogShipmateConsignmentRequestResponse", parameters, out rowsAffected);
        }

        public override ShipmateConsignmentRequestResponse MapDataToClass(System.Data.DataRow row)
        {
            ShipmateConsignmentRequestResponse s = new ShipmateConsignmentRequestResponse();
            return s;
        }
    }
}