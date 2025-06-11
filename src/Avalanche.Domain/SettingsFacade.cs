using Avalanche.ReadModel;
using Avalanche.ReadModel.Queries;
using Broadcast;
using System.Text.Json;

namespace Avalanche.Domain
{
    public class SettingsFacade : ISettingsFacade
    {
        private readonly ISettingsQueryHandler _queryHandler;
        private readonly IEventBus _eventBus;

        public SettingsFacade(ISettingsQueryHandler queryHandler, IEventBus eventBus)
        {
            _queryHandler = queryHandler;
            _eventBus = eventBus;
        }

        public void RecreateDatabase()
        {
            var events = _queryHandler.Get(new GetEventStoreEvents());
            var total = events.Count();
            var i = 1;
            foreach (var model in events.OrderBy(e => e.Time))
            {
                var evnt = JsonSerializer.Deserialize(model.Value, model.EventType);

                _eventBus.Send(evnt);

                Console.WriteLine($"Processed: {i}/{total}, Event: {model.EventType.Name}");
                i++;
            }
        }
    }
}
