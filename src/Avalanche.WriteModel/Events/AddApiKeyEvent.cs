using Broadcast;

namespace Avalanche.WriteModel.Events
{
    public class AddApiKeyEvent : IEvent
    {
        public string Name { get; set; }

        public string Value { get; set; }

        public DateTime Created { get; set; }

        public DateTime Expires { get; set; }
    }
}
