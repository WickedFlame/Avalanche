namespace Avalanche.ReadModel.Models
{
    public class TestSummary
    {
        public string TestId { get; set; }

        public string Type { get; set; }

        public int ThreadNumber { get; set; }

        public int Iterations { get; set; }

        public long AverageTicks { get; set; }

        public long TotalTime { get; set; }

        public long Fastest { get; set; }

        public long Slowest { get; set; }

        public long Increase { get; set; }

        public long InitialSize { get; set; }

        public long EndSize { get; set; }
    }
}
