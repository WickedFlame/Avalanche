using Avalanche.ReadModel.Models;
using Avalanche.ReadModel.Queries;
using System.Data.SQLite;

namespace Avalanche.ReadModel.QueryHandlers
{
    public class TestRunQueryHandler :
        IQueryHandler<IEnumerable<TestRun>, GetTestsQuery>,
        IQueryHandler<TestRun, GetLastTestQuery>
    {
        private readonly SQLiteConnection _connection;

        public TestRunQueryHandler()
        {
            _connection = new SQLiteConnection("Data Source=readmodel.db");
            _connection.Open();
        }

        public IEnumerable<TestRun> Get(GetTestsQuery query)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM TestRun WHERE Scenario = @scenario ORDER BY StartTime DESC";

                cmd.Parameters.Add(new SQLiteParameter("@scenario", query.Scenario));

                return cmd.Execute<TestRun>();
            }
        }

        public TestRun Get(GetLastTestQuery query)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM TestRun WHERE Scenario = @scenario ORDER BY StartTime DESC";

                cmd.Parameters.Add(new SQLiteParameter("@scenario", query.Scenario));

                return cmd.Execute<TestRun>().FirstOrDefault();
            }
        }
    }
}
