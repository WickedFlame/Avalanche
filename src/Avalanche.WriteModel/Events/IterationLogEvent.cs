using Broadcast;

namespace Avalanche.WriteModel.Events
{
    public class IterationLogEvent : IEvent
    {
        public DateTime Time { get; set; }

        public string TestId { get; set; }

        public string Name { get; set; }

        public int Thread { get; set; }

        public double Througput { get; set; }

        public int Iterations { get; set; }

        public double AverageMilliseconds { get; set; }
    }
}
