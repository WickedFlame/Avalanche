using Avalanche.WriteModel.Sqlite;
using SqlKata.Compilers;
using SqlKata.Execution;
using System.Data.SQLite;

namespace Avalanche.DataSource.Sqlite
{
    public class ProjectionConnectionBuilder : IProjectionConnectionBuilder
    {
        public QueryFactory Build()
        {
            var connection = new SQLiteConnection(Constants.ReadModelDatabase);
            var compiler = new SqliteCompiler();
            return new QueryFactory(connection, compiler);
        }
    }
}
