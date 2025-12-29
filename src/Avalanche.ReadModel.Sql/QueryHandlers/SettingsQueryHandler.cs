using Avalanche.DataSource;
using Avalanche.ReadModel.Models;
using Avalanche.ReadModel.Queries;
using SqlKata.Execution;

namespace Avalanche.ReadModel.Sql.QueryHandlers
{
    public class SettingsQueryHandler : ISettingsQueryHandler
    {
        private readonly QueryFactory _esdb;
        private readonly QueryFactory _db;

        public SettingsQueryHandler(IEventStoreConnectionBuilder esdb, IProjectionConnectionBuilder builder)
        {
            _esdb = esdb.Build();
            _db = builder.Build();
        }

        public IEnumerable<EventModel> Get(GetEventStoreEvents query)
        {
            return _esdb.Query(nameof(Avalanche.DataSource.DTO.Events))
                .Select()
                .Get<EventModel>();
        }

        public IEnumerable<ApiKey> Get(GetApiKeys query)
        {
            return _db.Query(nameof(Avalanche.DataSource.DTO.ApiKeys))
                .Select()
                .Get<ApiKey>();
        }

        public ApiKey Get(GetApiKey query)
        {
            return _db.Query(nameof(Avalanche.DataSource.DTO.ApiKeys))
                .Select()
                .Where("Name", query.Name)
                .Get<ApiKey>()
                .FirstOrDefault();
        }
    }
}
