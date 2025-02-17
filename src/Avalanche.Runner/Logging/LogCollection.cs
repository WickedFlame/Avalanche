using System;
using System.Collections.Generic;
using System.Text;

namespace Avalanche.Runner.Logging
{
    public class LogCollection
    {
        private readonly List<LogEvent> _events = new List<LogEvent>();

        public LogCollection(string name)
        {
            Name = name;
        }

        public string Name { get; }

        public string ThreadId { get; set; }

        public void Add(LogEvent metric)
        {
            _events.Add(metric);
        }

        public IEnumerable<LogEvent> Events => _events;
    }
}
