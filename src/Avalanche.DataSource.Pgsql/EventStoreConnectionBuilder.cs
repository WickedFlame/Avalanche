using Microsoft.Extensions.Configuration;
using Npgsql;
using SqlKata.Execution;

namespace Avalanche.DataSource.Pgsql
{
    public class EventStoreConnectionBuilder : ConnectionBuilder, IEventStoreConnectionBuilder
    {
        public EventStoreConnectionBuilder(IConfiguration config)
            : base("eventstore", config)
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
