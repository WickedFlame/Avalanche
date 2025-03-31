using Broadcast;

namespace Avalanche.WriteModel.Events
{
    public class IterationLogEvent : IEvent
    {
        public DateTime Time { get; set; }

        public string Category { get; set; }

        public string Module { get; set; }

        public string TestId { get; set; }

        public string Name { get; set; }

        public string Message { get; set; }

        public int RunNumber { get; set; }

        public int Thread { get; set; }

        public bool IsWarmup { get; set; }

        public double TotalMilliseconds { get; set; }
    }
}
