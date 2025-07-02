using Npgsql;
using SqlKata.Compilers;
using SqlKata.Execution;

namespace Avalanche.DataSource.Pgsql
{
    public class EventStoreConnectionBuilder : IEventStoreConnectionBuilder
    {
        public QueryFactory Build()
        {
            var connection = new NpgsqlConnection(Constants.EventStoreDatabase);
            var compiler = new AvalanchePostgresCompiler();
            return new QueryFactory(connection, compiler);
        }
    }
}
