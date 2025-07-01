using Avalanche.DataSource;
using Avalanche.ReadModel.Models;
using Avalanche.ReadModel.Queries;
using Avalanche.ReadModel.Sql;
using SqlKata.Execution;

namespace Avalanche.ReadModel.QueryHandlers
{
    public class TestRunQueryHandler : ITestRunQueryHandler
    {
        private readonly IProjectionConnectionBuilder _builder;

        public TestRunQueryHandler(IProjectionConnectionBuilder builder)
        {
            _builder = builder;
        }

        public IEnumerable<TestRun> Get(GetTestsQuery query)
        {
            var db = _builder.Build();
            return db.Query(nameof(DataSource.DTO.TestRun))
                .Select()
                .Where(new
                {
                    Scenario = query.Scenario,
                })
                .OrderByDesc("StartTime")
                .Get<TestRun>();
        }

        public TestRun Get(GetLastTestQuery query)
        {
            var db = _builder.Build();
            return db.Query(nameof(DataSource.DTO.TestRun))
                .Select()
                .Where(new
                {
                    Scenario = query.Scenario,
                })
                .OrderByDesc("StartTime")
                .FirstOrDefault<TestRun>();
        }

        public TestRun Get(GetTestRun query)
        {
            var db = _builder.Build();
            return db.Query(nameof(DataSource.DTO.TestRun))
                .Select()
                .Where(new
                {
                    TestId = query.TestId,
                })
                .OrderByDesc("StartTime")
                .FirstOrDefault<TestRun>();
        }

        public IEnumerable<RampupData> Get(GetRampupData query)
        {
            var db = _builder.Build();
            return db.Query(nameof(DataSource.DTO.RampupEvents))
                .Select()
                .Where(new
                {
                    TestId = query.TestId,
                    Name = query.TestName
                })
                .OrderByDesc("Time")
                .Get<RampupData>();
        }

        public IEnumerable<IterationItem> Get(GetChartData query)
        {
            var db = _builder.Build();
            return db.Query(nameof(DataSource.DTO.IterationEvents))
                .Select()
                .Where(new
                {
                    TestId = query.TestId,
                    TestName = query.TestName
                })
                .OrderByDesc("Time")
                .Get<IterationItem>();
        }

        public IEnumerable<TestSummary> Get(GetSummary query)
        {
            var db = _builder.Build();
            var summary = db.Query(nameof(DataSource.DTO.SummaryEvents))
                .Select()
                .Where(new
                {
                    TestId = query.TestId,
                    Type = "TestSummary"
                })
                .Get<TestSummary>()
                .ToList();

            var errors = db.Query(nameof(DataSource.DTO.IterationEvents))
                .Select()
                .Where(new
                {
                    TestId = query.TestId,
                    Error = true
                })
                .Get<IterationItem>();

            if (summary.Any())
            {
                foreach (var stat in summary)
                {
                    stat.Failed = errors.Count(e => !e.IsWarmup && e.TestName == stat.TestCase);
                }

                return summary;
            }

            // get configured test
            var details = db.Query(nameof(DataSource.DTO.TestRunDetail))
                .Select()
                .Where(new
                {
                    TestId = query.TestId
                })
                .Get<TestRunDetail>();

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

        public IEnumerable<TestStatistic> Get(GetTestsStatisticsQuery query)
        {
            var db = _builder.Build();
            var stats = db.Query(nameof(DataSource.DTO.TestRun))
                .Join(nameof(DataSource.DTO.SummaryEvents), "TestRun.TestId", "SummaryEvents.TestId")
                .Select()
                .Where(new
                {
                    Scenario = query.Scenario,
                    Type = "TestSummary"
                })
                .OrderByDesc("StartTime")
                .Get<TestStatistic>();


            var errors = db.Query(nameof(DataSource.DTO.TestRun))
                .Join(nameof(DataSource.DTO.IterationEvents), "TestRun.TestId", "IterationEvents.TestId")
                .Select()
                .Where(new
                {
                    Scenario = query.Scenario,
                    Error = true
                })
                .Get<IterationItem>();

            foreach (var stat in stats)
            {
                stat.Failed = errors.Count(e => !e.IsWarmup && e.TestId == stat.TestId && e.TestName == stat.TestCase);
            }

            return stats;
        }
    }
}
