namespace Avalanche.WriteModel.Events
{
    public class EndTestEvent : IEvent
    {
        public string TestId { get; set; }

        public DateTime EndTime { get; set; }

        public string Status { get; set; }
    }
}
