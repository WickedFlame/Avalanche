using Microsoft.Extensions.Configuration;
using SqlKata.Compilers;
using SqlKata.Execution;
using System.Data.SQLite;

namespace Avalanche.DataSource.Sqlite
{
    public class ProjectionConnectionBuilder : IProjectionConnectionBuilder
    {
        private readonly IConfiguration _config;

        public ProjectionConnectionBuilder(IConfiguration config)
        {
            _config = config;
        }

        public QueryFactory Build()
        {
            var builder = new ConnectionStringBuilder(Constants.ReadModel, _config);
            var connection = new SQLiteConnection(builder.BuildConnectionString());
            var compiler = new SqliteCompiler();
            return new QueryFactory(connection, compiler);
        }
    }
}
