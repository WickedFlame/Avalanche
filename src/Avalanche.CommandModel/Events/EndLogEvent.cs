
namespace Avalanche.CommandModel.Events
{
    public class EndLogEvent : LogEvent
    {
        public string Name { get; set; }

        public string Message { get; set; }

        public int RunNumber { get; set; }

        public int Thread { get; set; }

        public bool IsWarmup { get; set; }
    }
}
