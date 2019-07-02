using Mobile.Portal.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mobile.Portal.DAL
{
  public  interface IEngineerDataProvider
    {
      Engineer GetEngineerDetailsBySaediId(string SaediId, string loginSaediId);
    }
}
