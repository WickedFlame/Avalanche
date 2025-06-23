using Avalanche.DataSource;
using Avalanche.ReadModel.Queries;
using Broadcast;
using SqlKata.Execution;

namespace Avalanche.ReadModel.Sqlite.QueryHandlers
{
    public class SettingsQueryHandler : ISettingsQueryHandler
    {
        private readonly QueryFactory _db;

        public SettingsQueryHandler(IEventStoreConnectionBuilder builder)
        {
            _db = builder.Build();
        }

        public IEnumerable<EventModel> Get(GetEventStoreEvents query)
        {
            return _db.Query(nameof(Avalanche.DataSource.DTO.Events))
                .Select()
                .Get<EventModel>();
        }
    }
}
