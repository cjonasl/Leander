using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mobile.Portal.DAL;
using Mobile.Portal.Helpers;

namespace Mobile.Portal.BLL
{
  public  class OnlineBookingLogBLL
    {
      OnlineBookingLogDLL _dll;
      public OnlineBookingLogBLL()
      {
          _dll = new OnlineBookingLogDLL();
      }

      internal void InsertOnlineServiceLog(Object Request, Object Response, string SaediId,string Url, bool Success=false)
       {
         string Req =  StringHelper.Dump(Request);
         string Resp = StringHelper.Dump(Response);
         _dll.InsertOnlineServiceLog(Req, Resp, SaediId, Success,Url);

       }
    }
}
