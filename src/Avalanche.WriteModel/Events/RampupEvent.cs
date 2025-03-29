using System.Net;

namespace Avalanche.WriteModel.Events
{
    public class RampupEvent : IEvent
    {
        public RampupEvent()
        {
            Time = DateTime.Now;
        }

        public DateTime Time { get; set; }

        public string Category { get; set; }

        public string Module { get; set; }

        public string TestId { get; set; }

        public string Name { get; set; }

        public string Message { get; set; }

        public HttpStatusCode StatusCode { get; set; }

        public long ElapsedMilliseconds { get; set; }

        public bool IsWarmup { get; set; }
    }
}
