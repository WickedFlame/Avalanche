namespace Avalanche.WriteModel.Events
{
    public class EndLogEvent : LogEvent, IEvent
    {
        public string TestId { get; set; }

        public string Name { get; set; }

        public string Message { get; set; }

        public int RunNumber { get; set; }

        public int Thread { get; set; }

        public bool IsWarmup { get; set; }
    }
}
