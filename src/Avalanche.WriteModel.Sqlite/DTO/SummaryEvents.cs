namespace Avalanche.WriteModel.Sqlite.DTO
{
    public class SummaryEvents
    {
        public string Id { get; set; }

        public string TestId { get; set; }

        public DateTime Time { get; set; }

        public string TestCase { get; set; }

        public string Type { get; set; }

        public int Iterations { get; set; }

        public double AverageMilliseconds { get; set; }

        public double TotalMilliseconds { get; set; }

        public double Throughput {  get; set; }

        public double Slowest {  get; set; }

        public double Fastest { get; set; }
    }
}
