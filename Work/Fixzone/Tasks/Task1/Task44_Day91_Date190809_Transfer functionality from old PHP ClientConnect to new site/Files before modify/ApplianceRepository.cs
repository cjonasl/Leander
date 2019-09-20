using ClientConnect.Models.Appliance;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using Dapper;

namespace ClientConnect.Repositories
{
    public class ApplianceRepository : Repository
    {
        public TroubleshootModel GetApplianceTroubleShooxt(int troubleshootId)
        {

            using (SqlConnection connection = new SqlConnection(_dataContext.ConnectionString))
            {
                connection.Open();
                //using (var multi = connection.Query<TroubleshootModel, TroubleshootFault, TroubleshootDetail, TroubleshootModel>
                //    ("GetApplianceTroubleShoot", new { @troubleshootId = troubleshootId }, commandType: CommandType.StoredProcedure))
                //{

                //}
                var sql = @"SELECT c.TroubleshootID , c.TroubleshootCategory, f.TroubleshootFaultID,f.TroubleshootFault ,d.TroubleshootTitle,TroubleshoorText,TroubleshootQuestionID

	from troubleshootcategory C   join TroubleshootFault F on f.TroubleshootID=c.TroubleshootID
	join troubleshootdetail D on D.troubleshootid= c.troubleshootid   and d.TroubleshootFaultID = f.TroubleshootFaultID";//,IEnumerable<TroubleshootDetail>, 
                var identityMap = new Dictionary<int, TroubleshootModel>();
                 var identityMap1 = new Dictionary<int, TroubleshootFault>();
                var item = connection.Query<TroubleshootModel, TroubleshootFault, TroubleshootDetail,TroubleshootModel>(sql,
                    (p, c,d) => {
                       
                      

                        TroubleshootModel master;
                        if (!identityMap.TryGetValue(p.TroubleshootID, out master))
                        {
                            identityMap[p.TroubleshootID] = master = p;
                        }
                        var list = (List<TroubleshootFault>)master.TroubleshootFaultlist;
                        if (list == null)
                        {
                            master.TroubleshootFaultlist = list = new List<TroubleshootFault>();
                        }
                        list.Add(c); 
                        
                        
                        
                        
                        TroubleshootFault master1;
                        if (!identityMap1.TryGetValue(c.TroubleshootFaultID, out master1))
                        {
                            identityMap1[c.TroubleshootFaultID] = master1 = c;
                        }

                        var list1 = (List<TroubleshootDetail>)master1.troubleshootDetaillist;
                        if (list1 == null)
                        {
                            master1.troubleshootDetaillist = list1 = new List<TroubleshootDetail>();
                        }
                        list1.Add(d);

                        return master;
                    }, splitOn: "TroubleshootFaultID,TroubleshootQuestionID,TroubleshoorText").Where(x => x.TroubleshootID == troubleshootId).FirstOrDefault();

                return item;

} 

            }

        public TroubleshootModel GetApplianceTroubleShoot(int troubleshootId)
        {
            using (SqlConnection connection = new SqlConnection(_dataContext.ConnectionString))
            {
                connection.Open();
                TroubleshootModel troubleshootModel = new TroubleshootModel();


               var troubleshootModelList = connection.Query<TroubleshootModel>("select TroubleshootID , TroubleshootCategory  from TroubleshootCategory").ToList();
                troubleshootModel= troubleshootModelList.Where(x=> x.TroubleshootID == troubleshootId).FirstOrDefault();
                troubleshootModel.TroubleshootFaultlist = GetApplianceTroubleShootDetail(troubleshootId).OfType<TroubleshootFault>().ToList();
                return troubleshootModel;
            }
        
        }

        //public static IEnumerable<TOne> Query<TOne, TMany>(this IDbConnection cnn, string sql, Func<TOne, IList<TMany>> property, dynamic param = null, IDbTransaction transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null)
        //{
        //    var cache = new Dictionary<int, TOne>();
        //    cnn.Query<TOne, TMany, TOne>(sql, (one, many) =>
        //    {
        //        if (!cache.ContainsKey(one.GetHashCode()))
        //            cache.Add(one.GetHashCode(), one);

        //        var localOne = cache[one.GetHashCode()];
        //        var list = property(localOne);
        //        list.Add(many);
        //        return localOne;
        //    }, param as object, transaction, buffered, splitOn, commandTimeout, commandType);
        //    return cache.Values;
        //}
        //public TroubleshootModel GetApplianceTroubleShootDetail(int troubleshootId)
        //{
//            using (SqlConnection connection = new SqlConnection(_dataContext.ConnectionString))
//            {
//                connection.Open();
//                var query = @"SELECT  f.TroubleshootFaultID,f.TroubleshootFault ,d.TroubleshootTitle,TroubleshoorText,TroubleshootQuestionID
//
//	from  troubleshootdetail D   
//	join TroubleshootFault F on d.TroubleshootFaultID = f.TroubleshootFaultID and d.TroubleshootID=f.TroubleshootID
//	where  D.TroubleshootID=" + troubleshootId;

//                IEnumerable<TroubleshootFault> TroubleshootFaults = null;
//                using (var multi = connection.QueryMultiple(query))
//                {
//                    TroubleshootFaults = multi.Read<TroubleshootFault>
//                    { a.troubleshootDetaillist = g; return a; });
//                    if (TroubleshootFaults != null)
//                    {
//                        var TroubleshootDetails = multi.Read<TroubleshootDetail>().ToList();
//                        foreach (var TroubleshootFault in TroubleshootFaults)
//                        {
//                            TroubleshootFault.troubleshootDetaillist = TroubleshootDetails.Where(x => x. = TroubleshootFault.        ).ToList();
//                        }
//                    }
//                }

//  //           var test= QueryParentChild< TroubleshootFault, TroubleshootDetail,int>
//  //               (connection,"GetApplianceTroubleShoot",x => x.TroubleshootFaultID, x => x.troubleshootDetaillist,new {troubleshootId=troubleshootId}
//  //               ,null,true,,new {commandCommandType.StoredProcedure);




//            }

            public List<TroubleshootFault> GetApplianceTroubleShootDetail(int troubleshootId)
{


    var sql = @"
select troubleshootid, troubleshootfaultid,troubleshootfault from TroubleshootFault  where troubleshootid=@troubleshootId ;" +
"select troubleshootquestionid,troubleshootid,troubleshootfaultid,troubleshoottitle,troubleshoortext from troubleshootdetail where troubleshootid= @troubleshootId  ;";


    using (var connection =  new SqlConnection(_dataContext.ConnectionString))
    {
        connection.Open();


        var q = connection.QueryMultiple(sql, new { @troubleshootId = troubleshootId },
            //myParams,
                                     commandType: CommandType.Text);
        List<TroubleshootFault> TroubleshootFault = q.Read<TroubleshootFault>().OfType <TroubleshootFault>().ToList();

        List<TroubleshootDetail> TroubleshootDetail = q.Read<TroubleshootDetail>().OfType < TroubleshootDetail>().ToList();

        foreach (var item in TroubleshootFault)
        { 
            int i=TroubleshootFault.IndexOf(item);
            var TroubleshootDetails = TroubleshootDetail.Where(e => int.Equals(e.TroubleshootFaultID, item.TroubleshootFaultID)).Select(x => x);
           
            TroubleshootFault[i].troubleshootDetaillist=TroubleshootDetails.OfType<TroubleshootDetail>().ToList();

        }
     
     

        return TroubleshootFault;
    }
}



            internal void GetApplianceTroubleShoot(int custaplid, int troubleshootQid, int TroubleshootFaultId)
            {
                Execute("SaveSoftServiceJob",
                                        new
                                        {

                                            custaplid = custaplid,
                                            troubleshootQid = troubleshootQid,
                                            TroubleshootFaultId = TroubleshootFaultId
                                        }, commandType: CommandType.StoredProcedure);
            }
    }
}