using Avalanche.DataSource;
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
        private readonly IDataStoreBuilder _dataStoreBuilder;

        public SettingsFacade(ISettingsQueryHandler queryHandler, IEventBus eventBus, IDataStoreBuilder dataStoreBuilder)
        {
            _queryHandler = queryHandler;
            _eventBus = eventBus;
            _dataStoreBuilder = dataStoreBuilder;
        }

        public void RecreateDatabase()
        {
            _dataStoreBuilder.RecreateWriteModel();

            var events = _queryHandler.Get(new GetEventStoreEvents());
            var total = events.Count();
            var i = 1;
            foreach (var model in events.OrderBy(e => e.Time))
            {
                var type = Type.GetType(model.EventType);
                var evnt = JsonSerializer.Deserialize(model.Value, type);

                _eventBus.Send(evnt);

                Console.WriteLine($"Processed: {i}/{total}, Event: {type.Name}");
                i++;
            }
        }
    }
}
