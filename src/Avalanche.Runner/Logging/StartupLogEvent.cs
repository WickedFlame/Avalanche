
using System.Net;

namespace Avalanche.Runner.Logging
{
    public class StartupLogEvent : LogEvent
    {
        public string Name { get; set; }

        public string Message { get; set; }

        public HttpStatusCode StatusCode { get; set; }

        public long ElapsedMilliseconds { get; set; }

        public bool IsWarmup { get; set; }
    }
}
