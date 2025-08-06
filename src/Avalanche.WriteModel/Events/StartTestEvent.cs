using Broadcast;

namespace Avalanche.WriteModel.Events
{
    public class StartTestEvent : IEvent
    {
        public string TestId { get; set; }

        public string Scenario { get; set; }

        public DateTime StartTime { get; set; }

        public string Status { get; set; }

        public string Runner { get; set; }
    }
}
