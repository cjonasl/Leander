using System;
using System.Collections.Generic;
using System.Text;

namespace Leander.Nr1.HarmonyEventing
{
    public interface INotification : IEvent
    {
        string Description { get; }
    }
}
