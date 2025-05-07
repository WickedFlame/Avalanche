using Broadcast;
using System;

namespace Avalanche.WriteModel.Memory
{
    public class EventStoreItem
    {
        public string Id { get; set; }

        public string TestId { get; set; }

        public DateTime Time { get; set; }

        public IEvent Model { get; set; }
    }
}
