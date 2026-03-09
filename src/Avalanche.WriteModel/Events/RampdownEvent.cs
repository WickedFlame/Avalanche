using Broadcast;

namespace Avalanche.WriteModel.Events
{
    public class RampdownEvent
    {
        public RampdownEvent()
        {
            Time = DateTime.Now;
        }

        public DateTime Time { get; set; }

        public string TestId { get; set; }

        public string TestCase { get; set; }

        public string Message { get; set; }

        public int RunNumber { get; set; }

        public int ThreadId { get; set; }

        public bool IsWarmup { get; set; }
    }
}
