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
        public string ResMessage { get; set; }
        public string ResConsignmentReference { get; set; }
        public string ResParcelReference { get; set; }
        public string ResCarrier { get; set; }
        public string ResServiceName { get; set; }
        public string ResTrackingReference { get; set; }
        public string ResCreatedBy { get; set; }
        public string ResCreatedWith { get; set; }
        public DateTime ResCreatedAt { get; set; }
        public string ResDeliveryName { get; set; }
        public string ResLine1 { get; set; }
        public string ResLine2 { get; set; }
        public string ResLine3 { get; set; }
        public string ResCity { get; set; }
        public string ResCounty { get; set; }
        public string ResPostcode { get; set; }
        public string ResCountry { get; set; }
        public string ResPdf { get; set; }
        public string ResZpl { get; set; }
        public string ResPng { get; set; }
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
                parameters[16] = new SqlParameter("@ResMessage", s.ResMessage);
                parameters[17] = new SqlParameter("@ResConsignmentReference", s.ResConsignmentReference);
                parameters[18] = new SqlParameter("@ResParcelReference", s.ResParcelReference);
                parameters[19] = new SqlParameter("@ResCarrier", s.ResCarrier);
                parameters[20] = new SqlParameter("@ResServiceName", s.ResServiceName);
                parameters[21] = new SqlParameter("@ResTrackingReference", s.ResTrackingReference);
                parameters[22] = new SqlParameter("@ResCreatedBy", s.ResCreatedBy);
                parameters[23] = new SqlParameter("@ResCreatedWith", s.ResCreatedWith);
                parameters[24] = new SqlParameter("@ResCreatedAt", s.ResCreatedAt);
                parameters[25] = new SqlParameter("@ResDeliveryName", s.ResDeliveryName);
                parameters[26] = new SqlParameter("@ResLine1", s.ResLine1);
                parameters[27] = new SqlParameter("@ResLine2", s.ResLine2);
                parameters[28] = new SqlParameter("@ResLine3", s.ResLine3);
                parameters[29] = new SqlParameter("@ResCity", s.ResCity);
                parameters[30] = new SqlParameter("@ResCounty", s.ResCounty);
                parameters[31] = new SqlParameter("@ResPostcode", s.ResPostcode);
                parameters[32] = new SqlParameter("@ResCountry", s.ResCountry);
                parameters[33] = new SqlParameter("@ResPdf", s.ResPdf);
                parameters[34] = new SqlParameter("@ResZpl", s.ResZpl);
                parameters[35] = new SqlParameter("@ResPng", s.ResPng);
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