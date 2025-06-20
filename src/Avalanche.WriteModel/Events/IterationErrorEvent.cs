using Broadcast;

namespace Avalanche.WriteModel.Events
{
    public class IterationErrorEvent : IEvent
    {
        public DateTime Time { get; set; }

        public string TestId { get; set; }

        public string TestName { get; set; }

        public int Thread { get; set; }

        public string StatusCode { get; set; }

        public string Message { get; set; }
    }
}
