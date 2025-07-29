using Broadcast;

namespace Avalanche.WriteModel.Events
{
    public class ThreadSummaryEvent : IEvent
    {
        public string TestId { get; set; }

        public string TestCase { get; set; }

        public int ThreadId { get; set; }

        public int Iterations { get; set; }

        public double AverageMilliseconds { get; set; }

        public double TotalMilliseconds { get; set; }

        public double Throughput { get; set; }
    }
}
