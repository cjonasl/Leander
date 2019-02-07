using System;
using System.Collections.Generic;
using System.Text;

namespace Leander.Nr1.HarmonyEventing
{
    public static class Notifications
    {
        public static void Publish<T>(T eventMessage) where T : INotification
        {
            eventMessage.TriggeredBy = Guid.NewGuid();
        }
    }
}
