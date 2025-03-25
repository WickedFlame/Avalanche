namespace Avalanche.ReadModel.Models
{
    public class IterationItem
    {
        public DateTime Time { get; set; }

        public string TestId { get; set; }

        public string Name { get; set; }

        public int RunNumber { get; set; }

        public int ThreadId { get; set; }

        public bool IsWarmup { get; set; }

        public double TotalMilliseconds { get; set; }
    }
}
