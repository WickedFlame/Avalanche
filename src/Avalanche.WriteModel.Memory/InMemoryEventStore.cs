using Broadcast;
using System;
using System.Collections.Generic;

namespace Avalanche.WriteModel.Memory
{
    public class InMemoryEventStore : IEventStore
    {
        private readonly List<EventStoreItem> _events = new List<EventStoreItem>();

        public string Add<T>(string testId, DateTime time, T model) where T : IEvent
        {
            var id = Guid.NewGuid().ToString();

            _events.Add(new EventStoreItem
            {
                Id = id,
                TestId = testId,
                Time = time,
                Model = model
            });

            return id;
        }
    }
}
