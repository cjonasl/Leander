using Mobile.Portal.Classes;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Mobile.Portal.MSSQL;

namespace Mobile.Portal.DAL
{
    public class EngineerDataProvider : DataAccess<Engineer>, IEngineerDataProvider
    {
        public Engineer GetEngineerDetailsBySaediId(string SaediId, string loginSaediId)
        {
            SqlParameter[] parameters = { new SqlParameter("@SAEDIID", SaediId),
                                               new SqlParameter("@loginSaediId", loginSaediId)};
            return SelectStoredProc("GetEngineerDetailsBySaediId", parameters).FirstOrDefault();
        }

                     
        public override Engineer MapDataToClass(System.Data.DataRow row)
        {
            Engineer c = new Engineer();

          //  c.Id = int.Parse(row["SAEDIContactID"].ToString());
            c.SaediSourceId = row["SaediSourceId"].ToString();
            c.Pc_long = row["Pc_long"].ToString();
            c.name = row["name"].ToString();
            c.EngSaediId = row["EngSaediId"].ToString();
            return c;
        }
    }
}
