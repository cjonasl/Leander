using System;
using System.Collections.Generic;
using System.Text;

namespace Leander.Nr1.HarmonyEventing
{
    [Serializable]
    public abstract class Event : IEvent
    {
        protected Event()
        {
            EventId = Guid.NewGuid();
            ContextDescription = this.GetType().Name;
        }

        public virtual Guid EventId { get; set; }

        // The user who triggered the event
        public virtual Guid TriggeredBy { get; set; }

        public virtual string ContextDescription { get; set; }
    }
}
