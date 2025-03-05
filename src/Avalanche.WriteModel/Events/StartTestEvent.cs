namespace Avalanche.WriteModel.Events
{
    public class StartTestEvent : IEvent
    {
        public string TestId { get; set; }

        public string TestName { get; set; }

        public DateTime StartTime { get; set; }

        public string Status { get; set; }
    }
}
