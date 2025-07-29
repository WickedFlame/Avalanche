namespace Avalanche.ReadModel.Models
{
    public class TestRunDetail
    {
        public string TestId { get; set; }

        public string TestCase { get; set; }

        public int ThreadId { get; set; }

        public double Throughput { get; set; }

        public int Iterations { get; set; }

        public long AverageMilliseconds { get; set; }
    }
}
