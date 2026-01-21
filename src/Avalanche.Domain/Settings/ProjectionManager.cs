using Broadcast;

namespace Avalanche.Domain.Settings
{
    public class ProjectionManager
    {
        private readonly IEventStore _eventStore;
        private readonly IEventBus _eventBus;

        public ProjectionManager(IEventStore eventStore, IEventBus eventBus)
        {
            _eventStore = eventStore;
            _eventBus = eventBus;
        }

        public async Task ReplayAllEventsAsync()
        {
            var events = await _eventStore.ReadAllAsync();
            var total = events.Count();
            
            var i = 1;
            foreach (var evnt in events.OrderBy(e => e.Time))
            {
                _eventBus.Send(evnt.Data);

                Console.WriteLine($"Processed: {i}/{total}, Event: {evnt.Type}");
                i++;
            }
        }
    }
}
