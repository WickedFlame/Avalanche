using Broadcast;

namespace Avalanche.WriteModel.Events
{
    public class IterationErrorEvent
    {
        public DateTime Time { get; set; }

        public string TestId { get; set; }

        public string TestCase { get; set; }

        public int ThreadId { get; set; }

        public string StatusCode { get; set; }

        public string Message { get; set; }

        public bool IsWarmup { get; set; }
    }
}
