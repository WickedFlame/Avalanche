using Avalanche.DataSource.DTO;
using Avalanche.DataSource.Sqlite;
using Avalanche.ReadModel.Queries;
using Avalanche.ReadModel.QueryHandlers;
using Moq;
using NUnit.Framework.Internal;
using Polaroider;
using SqlKata.Compilers;
using SqlKata.Execution;
using System.Data.SQLite;
using System.Threading;

namespace Avalanche.ReadModel.Sqlite.IntegrationTests
{
    public class TestRunQueryHandlerTests
    {
        private QueryFactory _db;
        private TestRunQueryHandler _handler;

        [SetUp]
        public void Setup()
        {
            _handler = new TestRunQueryHandler(new ProjectionConnectionBuilder());
        }

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            var connection = new SQLiteConnection(Constants.ReadModelDatabase);
            var compiler = new SqliteCompiler();
            _db = new QueryFactory(connection, compiler);

            SetupTestData();
        }

        [TearDown]
        public void Teardown()
        {
            _db.Dispose();
        }

        [Test]
        public void TestRunQueryHandler_GetTestsQuery()
        {
            var tests = _handler.Get(new Queries.GetTestsQuery
            {
                Scenario = "Scenario 1"
            });
            tests.Should().HaveCount(2).And.AllBeEquivalentTo(new { Scenario = "Scenario 1" });
        }

        [Test]
        public void TestRunQueryHandler_GetLastTestQuery()
        {
            var test = _handler.Get(new Queries.GetLastTestQuery
            {
                Scenario = "Scenario 1"
            });
            test.TestId.Should().Be("3");
        }

        [Test]
        public void TestRunQueryHandler_GetTestRun()
        {
            var test = _handler.Get(new Queries.GetTestRun
            {
                TestId = "2"
            });
            test.TestId.Should().Be("2");
        }

        [Test]
        public void TestRunQueryHandler_GetRampupData()
        {
            var test = _handler.Get(new Queries.GetRampupData
            {
                TestId = "1",
                TestName = "Test 1"
            });

            test.Count(t => t.Value > 0).Should().Be(1);
            test.Count(t => t.Value < 0).Should().Be(1);
        }

        [Test]
        public void TestRunQueryHandler_GetChartData()
        {
            var test = _handler.Get(new Queries.GetChartData
            {
                TestId = "1",
                TestName = "Test 1"
            });
            test.Should().HaveCount(1);
        }








        [Test]
        public void TestRunQueryHandler_GetSummary()
        {
            var test = _handler.Get(new Queries.GetSummary
            {
                TestId = "2"
            });
            test.Should().HaveCount(1);
            test.Single().MatchSnapshot(SnapshotOptions.Create(o => o.MockDateTimes().MockGuids()));
        }

        [Test]
        public void TestRunQueryHandler_GetSummary_TestRunDetail()
        {
            var test = _handler.Get(new Queries.GetSummary
            {
                TestId = "1"
            });
            test.Should().HaveCount(1);
            test.Single().MatchSnapshot(SnapshotOptions.Create(o => o.MockDateTimes().MockGuids()));
        }


        [Test]
        public void TestRunQueryHandler_GetTestsStatisticsQuery()
        {
            var test = _handler.Get(new Queries.GetTestsStatisticsQuery
            {
                Scenario = "Scenario 2"
            });
            test.Should().HaveCount(1);
            test.Single().MatchSnapshot(SnapshotOptions.Create(o => o.MockDateTimes().MockGuids()));
        }








        private void SetupTestData()
        {
            _db.Query(nameof(TestRun))
                .Insert(new
                {
                    TestId = "1",
                    Scenario = "Scenario 1",
                    StartTime = DateTime.Now.AddMinutes(-5),
                    EndTime = DateTime.Now.AddMinutes(-3),
                    Status = "Ended"
                });

            _db.Query(nameof(TestRun))
                .Insert(new
                {
                    TestId = "2",
                    Scenario = "Scenario 2",
                    StartTime = DateTime.Now.AddMinutes(-5),
                    EndTime = DateTime.Now.AddMinutes(-3),
                    Status = "Ended"
                });

            _db.Query(nameof(TestRun))
                .Insert(new
                {
                    TestId = "3",
                    Scenario = "Scenario 1",
                    StartTime = DateTime.Now.AddMinutes(-2),
                    Status = "Started"
                });




            _db.Query(nameof(RampupEvents))
                .Insert(new
                {
                    Time = DateTime.Now.AddMinutes(-5),
                    TestId = "1",
                    //Scenario = "Scenario 1",                    
                    Name = "Test 1",
                    Value = 1
                });

            _db.Query(nameof(RampupEvents))
                .Insert(new
                {
                    Time = DateTime.Now.AddMinutes(-3),
                    TestId = "1",
                    //Scenario = "Scenario 1",
                    Name = "Test 1",
                    ThreadId = 1,
                    Value = -1
                });

            _db.Query(nameof(RampupEvents))
                .Insert(new
                {
                    Time = DateTime.Now.AddMinutes(-3),
                    TestId = "2",
                    //Scenario = "Scenario 1",
                    Name = "Test 1",
                    ThreadId = 1,
                    Value = 1
                });




            _db.Query(nameof(IterationEvents))
                .Insert(new
                {
                    Time = DateTime.Now.AddMinutes(-3),
                    TestId = "1",
                    TestName = "Test 1",
                    ThreadId = 1
                });
            _db.Query(nameof(IterationEvents))
                .Insert(new
                {
                    Time = DateTime.Now.AddMinutes(-2),
                    TestId = "1",
                    TestName = "Test 2",
                    ThreadId = 1
                });
            _db.Query(nameof(IterationEvents))
                .Insert(new
                {
                    Time = DateTime.Now.AddMinutes(-3),
                    TestId = "2",
                    TestName = "Test 1",
                    ThreadId = 1
                });
            _db.Query(nameof(IterationEvents))
                .Insert(new
                {
                    Time = DateTime.Now.AddMinutes(-2),
                    TestId = "2",
                    TestName = "Test 1",
                    ThreadId = 1,
                    Error = true
                });

            _db.Query(nameof(TestRunDetail))
                .Insert(new
                {
                    TestId = "1",
                    ThreadId = 1,
                    Iterations = 10,
                    Throughput = 2.1
                });
            _db.Query(nameof(TestRunDetail))
                .Insert(new
                {
                    TestId = "1",
                    ThreadId = 2,
                    Iterations = 10,
                    Throughput = 2.1
                });


            _db.Query(nameof(SummaryEvents))
                .Insert(new
                {
                    TestId = "2",
                    Time = DateTime.Now.AddMinutes(-2),
                    TestCase = "Test 1",
                    Type = "ThreadSummary",
                    Iterations = 50,
                    AverageMilliseconds = 123,
                    TotalMilliseconds = 321,
                    Throughput = 2.1,
                    Slowest = 51.2,
                    Fastest = 21.5
                });
            _db.Query(nameof(SummaryEvents))
                .Insert(new
                {
                    TestId = "2",
                    Time = DateTime.Now.AddMinutes(-2),                    
                    TestCase = "Test 1",
                    Type = "TestSummary",
                    Iterations = 50,
                    AverageMilliseconds = 123,
                    TotalMilliseconds = 321,
                    Throughput = 2.1,
                    Slowest = 51.2,
                    Fastest = 21.5
                });

        }
    }
}