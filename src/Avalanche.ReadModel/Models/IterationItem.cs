namespace Avalanche.ReadModel.Models
{
    public class IterationItem
    {
        public DateTime Time { get; set; }

        public string TestId { get; set; }

        public string TestName { get; set; }

        public int ThreadId { get; set; }

        public bool IsWarmup { get; set; }

        public double Throughput { get; set; }

        public double AverageMilliseconds { get; set; }
    }
}
