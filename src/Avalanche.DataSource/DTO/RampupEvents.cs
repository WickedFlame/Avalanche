using System;

namespace Avalanche.DataSource.DTO
{
    public class RampupEvents
    {
        public string Id { get; set; }

        public string TestId { get; set; }

        public string Name { get; set; }

        public DateTime Time { get; set; }

        public int ThreadId { get; set; }

        public int Value { get; set; }
    }
}
