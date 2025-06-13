using Broadcast;

namespace Avalanche.WriteModel.Events
{
    public class DeleteTestRunEvent : IEvent
    {
        public string TestId { get; set; }
    }
}
