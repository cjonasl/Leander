using System;
using System.Collections.Generic;
using System.Text;

namespace Leander.Nr1.HarmonyEventing
{
    [Serializable]
    public abstract class Notification : Event, INotification
    {
        public abstract string Description { get; }
    }
}
