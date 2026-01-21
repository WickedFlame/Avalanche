using Avalanche.DataSource;
using Avalanche.ReadModel;
using Avalanche.ReadModel.Models;
using Avalanche.ReadModel.Queries;
using Avalanche.WriteModel.Events;
using Broadcast;
using System.Security.Cryptography;
using System.Text.Json;

namespace Avalanche.Domain.Settings
{
    public class SettingsFacade : ISettingsFacade
    {
        private readonly ISettingsQueryHandler _queryHandler;
        private readonly IEventBus _eventBus;
        private readonly IDataStoreBuilder _dataStoreBuilder;
        private readonly ProjectionManager _projectionManager;

        public SettingsFacade(ISettingsQueryHandler queryHandler, IEventBus eventBus, IDataStoreBuilder dataStoreBuilder, ProjectionManager projectionManager)
        {
            _queryHandler = queryHandler;
            _eventBus = eventBus;
            _dataStoreBuilder = dataStoreBuilder;
            _projectionManager = projectionManager;
        }

        public void RecreateDatabase()
        {
            _dataStoreBuilder.RecreateWriteModel();

            //var events = _queryHandler.Get(new GetEventStoreEvents());
            //var total = events.Count();
            //var i = 1;
            //foreach (var model in events.OrderBy(e => e.Time))
            //{
            //    var type = Type.GetType(model.EventType);
            //    var evnt = JsonSerializer.Deserialize(model.Value, type);

            //    _eventBus.Send(evnt);

            //    Console.WriteLine($"Processed: {i}/{total}, Event: {type.Name}");
            //    i++;
            //}

            _projectionManager.ReplayAllEventsAsync().GetAwaiter().GetResult();
        }

        public string AddApiKey(string name, Expiration expiration)
        {
            var apiKey = GenerateApiKey();
            var expires = expiration switch
            {
                Expiration.OneMonth => DateTime.Now.AddMonths(1),
                Expiration.ThreeMonths => DateTime.Now.AddMonths(3),
                Expiration.SixMonths => DateTime.Now.AddMonths(6),
                Expiration.TwelveMonths => DateTime.Now.AddMonths(12),
                _ => DateTime.MinValue
            };

            _eventBus.Send(new AddApiKeyEvent { Name = name, Value = apiKey, Created = DateTime.Now, Expires = expires });

            return apiKey;
        }

        public void DeleteApiKey(string name)
        {
            _eventBus.Send(new DeleteApiKeyEvent { Name = name });
        }

        public ApiKey GetApiKey(string name)
        {
            return _queryHandler.Get(new GetApiKey { Name = name });
        }


        public IEnumerable<ApiKey> GetApiKeys()
        {
            return _queryHandler.Get(new GetApiKeys());
        }


        private const string _prefix = "Avlch-";
        private const int _numberOfSecureBytesToGenerate = 32;
        private const int _lengthOfKey = 32;

        private static string GenerateApiKey()
        {
            var bytes = RandomNumberGenerator.GetBytes(_numberOfSecureBytesToGenerate);

            var base64String = Convert.ToBase64String(bytes)
                .Replace("+", "-")
                .Replace("/", "_");

            var keyLength = _lengthOfKey - _prefix.Length;

            return _prefix + base64String[..keyLength];
        }
    }
}
