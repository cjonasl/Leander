using System;
using System.Collections.Generic;
using System.Text;

namespace Leander.Nr1.HarmonyEventing
{
    public interface IEvent : IMessage
    {
        Guid EventId { get; set; }
        Guid TriggeredBy { get; set; }
    }
}
