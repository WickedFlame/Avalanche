
namespace Avalanche.Runner.Logging
{
    public class IterationLogEvent : LogEvent
    {
        public string Name { get; set; }

        public string Message { get; set; }

        public int RunNumber { get; set; }

        public int Thread { get; set; }

        public bool IsWarmup { get; set; }

        public double TotalMilliseconds { get; set; }
    }
}
