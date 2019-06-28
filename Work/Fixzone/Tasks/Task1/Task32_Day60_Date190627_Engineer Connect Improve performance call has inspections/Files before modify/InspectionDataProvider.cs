using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using Mobile.Portal.Classes;

namespace Mobile.Portal.DAL
{
    public class InspectionDataProvider : DataAccess<Inspection>, Mobile.Portal.DAL.IInspectionDataProvider
    {
        public List<Inspection> GetForCall(string fromId, string toId, string clientRef)
        {
            SqlParameter[] parameter = new SqlParameter[] {  
                new SqlParameter("@fromID", fromId), 
                new SqlParameter("@toID", toId),
                new SqlParameter("@clientRef", clientRef) };
            List<Inspection> list = SelectSqlStatement("SELECT * FROM Inspection WHERE ((FROMID = @fromId AND TOID = @toId) OR (FROMID = @toId AND TOID = @fromId)) AND CLIENTREF = @clientRef ORDER BY reference", parameter);
            //List<Inspection> Custlist = SelectSqlStatement("SELECT * FROM CustInspection WHERE ((FROMID = @fromId AND TOID = @toId) OR (FROMID = @toId AND TOID = @fromId)) AND CLIENTREF = @clientRef ORDER BY reference", parameter);
            //list.AddRange(Custlist);
            return list;
        }

        public Boolean Update(int uid, int status)
        {
            SqlParameter[] parameter = new SqlParameter[] { 
                new SqlParameter("@uid", uid), 
                new SqlParameter("@status", status)
            };
            int rows = 0;
            rows = ExecuteSqlStatement("UPDATE Inspection SET status = @status WHERE _id = @uid", parameter, rows);
            return true;
        }

        public void Update(Inspection inspection)
        {
            InspectionDataDataProvider dataDAL = new InspectionDataDataProvider();
            foreach (InspectionData data in inspection.Data)
            {
                dataDAL.Update(data.UID,data.Response,data.Status);
            }            
            Update(inspection.uid, inspection.Status);
        }

        public Inspection GetByReference(string fromId, string toId, string clientRef, int reference)
        {
            SqlParameter[] parameter = new SqlParameter[] {  
                new SqlParameter("@fromID", fromId), 
                new SqlParameter("@toID", toId),
                new SqlParameter("@clientRef", clientRef),
                new SqlParameter("@reference", reference)
            };
            List<Inspection> list = SelectSqlStatement("SELECT * FROM Inspection WHERE FROMID = @fromId AND TOID = @toId AND CLIENTREF = @clientRef AND reference = @reference", parameter);

            //List<Inspection> Custlist = SelectSqlStatement("SELECT * FROM CustInspection WHERE ((FROMID = @fromId AND TOID = @toId) OR (FROMID = @toId AND TOID = @fromId)) AND CLIENTREF = @clientRef ORDER BY reference", parameter);
            //list.AddRange(Custlist);
            if (list.Count > 0)
            { return list[0]; }
            else
            { return null; }
        }

        public override Inspection MapDataToClass(System.Data.DataRow row)
        {
            InspectionDataDataProvider dataDAL = new InspectionDataDataProvider();
            Inspection item = new Inspection();
            try
            {
                item.uid = Int32.Parse(row["_id"].ToString());
                item.SaediFromId = row["fromID"].ToString();
                item.SaediToId = row["toID"].ToString();
                item.ClientRef = row["clientRef"].ToString();
                item.Name = row["name"].ToString();
                item.Reference = Int32.Parse(row["reference"].ToString());
                item.InspectionId = int.Parse(row["InspectionId"].ToString());
                List<InspectionData> inspectionData = dataDAL.GetDataForInspection(Int32.Parse(row["_id"].ToString()));

                if (item.SaediFromId.ToUpper() == "SONY3C")
                {
                    List<InspectionData> result = new List<InspectionData>();
                    foreach (InspectionData data in inspectionData)
                    {
                        data.IsSony = true;
                        result.Add(data);

                    }
                    item.Data = result;
                }
                else
                    item.Data = inspectionData;
            }
            catch (Exception ex)
            {
            }
           return item;
        }


        public List<string> GetInspectionDDContent(string table, string columnName, string filter)
        {
            SqlParameter[] parameters = new SqlParameter[] { };
            //SqlParameter[] parameters = { new SqlParameter("@id", id) ,
            //                  new SqlParameter("@EntityId", EntityId) ,
            //                  new SqlParameter("@apptypeid", apptypeid),
            //                                     new SqlParameter("@manufactid", manufactid)  ,
            //                              new SqlParameter("@PageNumber", PageIndex),
            //                            new SqlParameter("@PNC", PNC)  ,
            //                            new SqlParameter("@Stockcategory", Stockcategory) ,
            //                                  new SqlParameter("@dtdetails", dtdetails)};
            //return (MSSQLAccess.SelectStoredProcedure("[FZ_GetModelDetailsByModelID]", parameters));
            //SqlParameter[] parameter = new SqlParameter[] {  
            //    new SqlParameter("@fromID", fromId), 
            //    new SqlParameter("@toID", toId),
            //    new SqlParameter("@clientRef", clientRef),
            //    new SqlParameter("@reference", reference)
            //};
            List<string> list = SelectSQLStatement(string.Format("SELECT [{0}] FROM {1} where {2}", columnName, table, filter), parameters);
            return list;

        }
    }
}
