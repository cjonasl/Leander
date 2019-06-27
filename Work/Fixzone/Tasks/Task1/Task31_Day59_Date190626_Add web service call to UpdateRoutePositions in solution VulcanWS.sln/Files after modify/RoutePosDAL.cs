using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vulcan.WebService;

namespace Vulcan.DAL
{
  public  class RoutePosDAL
    {
        public int UpdateRoutePosCall(string fromId, List<Classes.TRoutePos> list, string username, string password)
        {
            OnlineBooking onlineBooking = new OnlineBooking();
            return onlineBooking.UpdateRoutePositions(list);
        }
    }
}
