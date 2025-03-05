using System;
using System.Collections.Generic;
using System.Text;

namespace Avalanche.WriteModel.Events
{
    public class LogEvent : IEvent
    {
        public LogEvent()
        {
            Time = DateTime.Now;
        }

        public DateTime Time { get; set; }

        public string Category { get; set; }

        public string Module { get; set; }
    }
}
