using Broadcast;

namespace Avalanche.WriteModel.Events
{
    public class RampupEvent : IEvent
    {
        public RampupEvent()
        {
            Time = DateTime.Now;
        }

        public DateTime Time { get; set; }

        public string TestId { get; set; }

        public string TestCase { get; set; }

        public long ElapsedMilliseconds { get; set; }

        public bool IsWarmup { get; set; }
    }
}
