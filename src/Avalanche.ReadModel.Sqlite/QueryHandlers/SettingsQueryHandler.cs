using Avalanche.ReadModel.Queries;
using Broadcast;
using System.Data.SQLite;

namespace Avalanche.ReadModel.Sqlite.QueryHandlers
{
    public class SettingsQueryHandler : ISettingsQueryHandler
    {
        public IEnumerable<EventModel> Get(GetEventStoreEvents query)
        {
            var connection = new SQLiteConnection(Constants.EventStoreDatabase);
            connection.Open();
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM Events";

                return cmd.Execute<EventModel>();
            }
        }
    }
}
