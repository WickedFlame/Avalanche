using Npgsql;
using SqlKata.Execution;

namespace Avalanche.DataSource.Pgsql
{
    public class EventStoreConnectionBuilder : ConnectionBuilder, IEventStoreConnectionBuilder
    {
        public EventStoreConnectionBuilder()
            : base("eventstore")
        {
        }

        public QueryFactory Build()
        {
            var connection = new NpgsqlConnection(base.BuildConnectionString());
            var compiler = new AvalanchePostgresCompiler();
            return new QueryFactory(connection, compiler);
        }
    }
}
