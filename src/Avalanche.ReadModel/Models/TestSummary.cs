namespace Avalanche.ReadModel.Models
{
    public class TestSummary
    {
        public string TestId { get; set; }

        public string TestCase { get; set; }

        public string Type { get; set; }

        public int ThreadNumber { get; set; }

        public int Iterations { get; set; }

        public double AverageMilliseconds { get; set; }

        public double TotalMilliseconds { get; set; }

        public double Throughput { get; set; }
    }
}
