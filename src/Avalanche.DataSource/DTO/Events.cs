using System;

namespace Avalanche.DataSource.DTO
{
    public class Events
    {
        public string Id { get; set; }

        public string StreamId { get; set; }

        public int StreamVersion { get; set; }

        public string EventType { get; set; }

        public DateTime Time { get; set; }

        public string Data {  get; set; }
    }
}
