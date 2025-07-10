namespace Avalanche.ReadModel.Models
{
    public class TestSummary
    {
        public string TestId { get; set; }

        public string TestCase { get; set; }

        public string Type { get; set; }

        public int ThreadId { get; set; }

        public int Iterations { get; set; }

        public double AverageMilliseconds { get; set; }

        public double TotalMilliseconds { get; set; }

        public double Throughput { get; set; }

        public int Failed { get; set; }

        public double Slowest { get; set; }

        public double Fastest { get; set; }
    }
}
