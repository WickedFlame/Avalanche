using Avalanche.ReadModel.Models;
using Avalanche.ReadModel.Queries;
using Avalanche.ReadModel.Sqlite;
using System.Data.SQLite;

namespace Avalanche.ReadModel.QueryHandlers
{
    public class TestRunQueryHandler :
        IQueryHandler<IEnumerable<TestRun>, GetTestsQuery>,
        IQueryHandler<IEnumerable<TestStatistic>, GetTestsStatisticsQuery>,
        IQueryHandler<TestRun, GetLastTestQuery>,
        IQueryHandler<TestRun, GetTestRun>,
        IQueryHandler<IEnumerable<RampupData>, GetRampupData>,
        IQueryHandler<IEnumerable<TestSummary>, GetSummary>
    {
        private readonly SQLiteConnection _connection;

        public TestRunQueryHandler()
        {
            _connection = new SQLiteConnection(Constants.ReadModelDatabase);
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
                cmd.CommandText = "SELECT * FROM IterationEvents WHERE TestId = @testId AND TestName = @name ORDER BY Time DESC";

                cmd.Parameters.Add(new SQLiteParameter("@testId", query.TestId));
                cmd.Parameters.Add(new SQLiteParameter("@name", query.TestName));

                return cmd.Execute<IterationItem>();
            }
        }

        public IEnumerable<TestSummary> Get(GetSummary query)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM SummaryEvents WHERE TestId = @testId AND Type = 'TestSummary'";

                cmd.Parameters.Add(new SQLiteParameter("@testId", query.TestId));

                var summary = cmd.Execute<TestSummary>().ToList();

                cmd.CommandText = "SELECT * FROM IterationEvents WHERE TestId = @testId AND Error = true";
                cmd.Parameters.Add(new SQLiteParameter("@testId", query.TestId));

                var errors = cmd.Execute<IterationItem>();

                if (summary.Any())
                {
                    foreach (var stat in summary)
                    {
                        stat.Failed = errors.Count(e => !e.IsWarmup && e.TestName == stat.TestCase);
                    }

                    return summary;
                }

                // get configured test
                cmd.CommandText = "SELECT * FROM TestRunDetail WHERE TestId = @testId";
                cmd.Parameters.Add(new SQLiteParameter("@testId", query.TestId));

                var details = cmd.Execute<TestRunDetail>();
                summary.AddRange(details
                    .GroupBy(d => d.TestCase)
                    .Select(detail => new TestSummary
                    {
                        TestId = query.TestId,
                        TestCase = detail.Key,
                        Throughput = detail.Sum(d => d.Throughput) / detail.Count(),
                        Iterations = detail.Sum(d => d.Iterations),
                        Type = "TestSummary",
                        Failed = errors.Count(e => !e.IsWarmup && e.TestName == detail.Key),
                        Slowest = 0,
                        Fastest = 0
                    }));

                return summary;
            }
        }

        public IEnumerable<TestStatistic> Get(GetTestsStatisticsQuery query)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM TestRun tr INNER JOIN SummaryEvents se ON tr.TestId = se.TestId WHERE tr.Scenario = @scenario AND se.Type = 'TestSummary' ORDER BY StartTime DESC";

                cmd.Parameters.Add(new SQLiteParameter("@scenario", query.Scenario));

                var stats = cmd.Execute<TestStatistic>();


                cmd.CommandText = "SELECT * FROM TestRun tr INNER JOIN IterationEvents ie ON tr.TestId = ie.TestId WHERE tr.Scenario = @scenario AND Error = true";
                cmd.Parameters.Add(new SQLiteParameter("@scenario", query.Scenario));

                var errors = cmd.Execute<IterationItem>();

                foreach (var stat in stats)
                {
                    stat.Failed = errors.Count(e => !e.IsWarmup && e.TestId == stat.TestId && e.TestName == stat.TestCase);
                }

                return stats;
            }
        }
    }
}
