using System;

namespace Avalanche.DataSource.DTO
{
    public class IterationEvents
    {
        public string Id { get; set; }

        public DateTime Time { get; set; }

        public string TestId { get; set; }

        public string TestName { get; set; }

        public int Thread { get; set; }

        public double Throughput { get; set; }

        public int Iterations { get; set; }

        public double AverageMilliseconds { get; set; }

        public bool IsWarmup { get; set; }

        public string Message { get; set; }

        public string StatusCode { get; set; }

        public bool Error { get; set; }
    }
}
