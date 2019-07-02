using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mobile.Portal.Classes;
using Mobile.Portal.DAL;

namespace Mobile.Portal.BLL
{
    public class EngineerBLL : BaseBLL<Engineer>
    {
        IEngineerDataProvider _dal;
        // no Data Acces just used for views
        public EngineerBLL()
        {
            _dal = new EngineerDataProvider();
        }

        public Engineer GetEngineerDetailsBySaediId(string SaediId, string loginSaediId)
        {
            return _dal.GetEngineerDetailsBySaediId(SaediId, loginSaediId);
        }

        public bool SonySonarEngineerIsActive(string SaediId)
        {
            return _dal.SonySonarEngineerIsActive(SaediId);
        }
    }
}
