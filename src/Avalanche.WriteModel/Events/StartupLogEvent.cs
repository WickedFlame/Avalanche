using System.Net;

namespace Avalanche.WriteModel.Events
{
    public class StartupLogEvent : LogEvent, IEvent
    {
        public string Name { get; set; }

        public string Message { get; set; }

        public HttpStatusCode StatusCode { get; set; }

        public long ElapsedMilliseconds { get; set; }

        public bool IsWarmup { get; set; }
    }
}
