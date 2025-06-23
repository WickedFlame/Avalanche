using SqlKata.Compilers;
using SqlKata.Execution;
using System.Data.SQLite;

namespace Avalanche.DataSource.Sqlite
{
    public class EventStoreConnectionBuilder : IEventStoreConnectionBuilder
    {
        public QueryFactory Build()
        {
            var connection = new SQLiteConnection(Constants.EventStoreDatabase);
            var compiler = new SqliteCompiler();
            return new QueryFactory(connection, compiler);
        }
    }
}
