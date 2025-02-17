using System;
using System.Collections.Generic;
using System.Text;

namespace Avalanche.Runner.Logging
{
    public class LogEvent
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
