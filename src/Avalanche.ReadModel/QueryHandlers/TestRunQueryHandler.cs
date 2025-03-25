using Avalanche.ReadModel.Models;
using Avalanche.ReadModel.Queries;
using System.Data.SQLite;

namespace Avalanche.ReadModel.QueryHandlers
{
    public class TestRunQueryHandler :
        IQueryHandler<IEnumerable<TestRun>, GetTestsQuery>,
        IQueryHandler<TestRun, GetLastTestQuery>,
        IQueryHandler<TestRun, GetTestRun>,
        IQueryHandler<IEnumerable<RampupData>, GetRampupData>,
        IQueryHandler<IEnumerable<TestSummary>, GetSummary>
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

        public TestRun Get(GetTestRun query)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM TestRun WHERE TestId = @testId ORDER BY StartTime DESC";

                cmd.Parameters.Add(new SQLiteParameter("@testId", query.TestId));

                return cmd.Execute<TestRun>().FirstOrDefault();
            }
        }

        public IEnumerable<RampupData> Get(GetRampupData query)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM RampupEvents WHERE TestId = @testId AND Name = @name ORDER BY Time DESC";

                cmd.Parameters.Add(new SQLiteParameter("@testId", query.TestId));
                cmd.Parameters.Add(new SQLiteParameter("@name", query.TestName));

                return cmd.Execute<RampupData>();
            }
        }

        public IEnumerable<IterationItem> Get(GetChartData query)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM IterationEvents WHERE TestId = @testId AND Name = @name ORDER BY Time DESC";

                cmd.Parameters.Add(new SQLiteParameter("@testId", query.TestId));
                cmd.Parameters.Add(new SQLiteParameter("@name", query.TestName));

                return cmd.Execute<IterationItem>();
            }
        }

        public IEnumerable<TestSummary> Get(GetSummary query)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM SummaryEvents WHERE TestId = @testId";

                cmd.Parameters.Add(new SQLiteParameter("@testId", query.TestId));

                return cmd.Execute<TestSummary>();
            }
        }
    }
}
