using Broadcast;

namespace Avalanche.WriteModel.Events
{
    public class DeleteApiKeyEvent : IEvent
    {
        public string Name { get; set; }
    }
}
