using Npgsql;
using SqlKata.Execution;

namespace Avalanche.DataSource.Pgsql
{
    public class ProjectionConnectionBuilder : IProjectionConnectionBuilder
    {
        public QueryFactory Build()
        {
            var connection = new NpgsqlConnection(Constants.ReadModelDatabase);
            var compiler = new AvalanchePostgresCompiler();
            return new QueryFactory(connection, compiler);
        }
    }
}
