using Broadcast;

namespace Avalanche.WriteModel.Events
{
    public class IterationLogEvent
    {
        public DateTime Time { get; set; }

        public string TestId { get; set; }

        public string TestCase { get; set; }

        public int Thread { get; set; }

        public double Throughput { get; set; }

        public int Iterations { get; set; }

        public double AverageMilliseconds { get; set; }

        public bool IsWarmup { get; set; }

        public int ContentLength { get; set; }
    }
}
