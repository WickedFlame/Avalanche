using System;

namespace Avalanche.QueryModel.Models
{
    public class TestRun
    {
        public string TestId { get; set; }

        public string TestName { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public string Status { get; set; }
    }
}
