using Broadcast;

namespace Avalanche.WriteModel.Events
{
    public class TestSummaryEvent : IEvent
    {
        public string TestId { get; set; }

        public int Users { get; set; }

        public int Iterations { get; set; }

        public double AverageMilliseconds { get; set; }

        public double TotalMilliseconds { get; set; }

        public string TestCase { get; set; }

        public double Throughput { get; set; }

        public double Slowest { get; set; }

        public double Fastest { get; set; }
    }
}
