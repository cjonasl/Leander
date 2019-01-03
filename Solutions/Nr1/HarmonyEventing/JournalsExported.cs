using System;
using System.Collections.Generic;
using System.Text;

namespace Leander.Nr1.HarmonyEventing
{
    [Serializable]
    public class JournalsExported : Notification
    {
        public override string Description
        {
            get { return "Journals Exported"; }
        }
    }
}
