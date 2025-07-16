using SqlKata.Compilers;
using SqlKata.Execution;
using System.Data.SQLite;

namespace Avalanche.DataSource.Sqlite
{
    public class ProjectionConnectionBuilder : IProjectionConnectionBuilder
    {
        public QueryFactory Build()
        {
            var builder = new ConnectionStringBuilder(Constants.ReadModel);
            var connection = new SQLiteConnection(builder.BuildConnectionString());
            var compiler = new SqliteCompiler();
            return new QueryFactory(connection, compiler);
        }
    }
}
