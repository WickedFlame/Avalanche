namespace Avalanche.ReadModel.Models
{
    public class TestStatistic
    {
        public string TestId { get; set; }

        public string TestCase { get; set; }

        /// <summary>
        /// Starttime of the Testrun
        /// </summary>
        public DateTime StartTime { get; set; }

        public DateTime Time { get; set; }

        public string Type { get; set; }

        public int Iterations { get; set; }

        public double AverageMilliseconds { get; set; }

        public double TotalMilliseconds { get; set; }

        public double Throughput { get; set; }
    }
}
