using System;

namespace Avalanche.DataSource.DTO
{
    public class Events
    {
        public string Id { get; set; }

        public string TestId { get; set; }

        public DateTime Time { get; set; }

        public string EventType { get; set; }

        public string Value {  get; set; }
    }
}
