using Npgsql;
using SqlKata.Execution;

namespace Avalanche.DataSource.Pgsql
{
    public class ProjectionConnectionBuilder : ConnectionBuilder, IProjectionConnectionBuilder
    {
        public ProjectionConnectionBuilder()
            : base("readmodel")
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
