using System;
using System.Collections.Generic;
using Cast.Sessions;
using System.Linq;
using System.Web;

namespace CAST.Services
{
    public class Service
    {
        private SessionHolder _session;

        /// <summary>
        /// Get session holder
        /// </summary>
        protected SessionHolder Session
        {
            get { return _session ?? (_session = new SessionHolder()); }
        }
    }
}