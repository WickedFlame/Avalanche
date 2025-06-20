using Avalanche.WriteModel.Events;
using Avalanche.WriteModel.Sqlite.EventHandlers;
using SqlKata.Compilers;
using SqlKata.Execution;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avalanche.WriteModel.Sqlite.IntegrationTests.EventHandlers
{
    public class SummaryEventHandlerTests
    {
        private QueryFactory _db;

        [SetUp]
        public void Setup()
        {
            var connection = new SQLiteConnection(Constants.ReadModelDatabase);
            var compiler = new SqliteCompiler();
            _db = new QueryFactory(connection, compiler);
        }

        [TearDown]
        public void Teardown()
        {
            _db.Dispose();
        }

        [Test]
        public void SummaryEventHandler_ThreadSummaryEvent()
        {
            var handler = new SummaryEventHandler(_db);
            handler.Handle(new ThreadSummaryEvent
            {
                TestId = "1",
                TestCase = "testcase 1",
                ThreadNumber = 1,
                Iterations = 1,
                AverageMilliseconds = 1,
                TotalMilliseconds = 1,
                Throughput = 1.1
            });

            var se = _db.Query(nameof(SummaryEvents))
                .Select()
                .Where("TestId", "1")
                .First<SummaryEvents>();

            se.TestCase.Should().Be("testcase 1");
            se.Type.Should().Be("ThreadSummary");
        }

        [Test]
        public void SummaryEventHandler_TestSummaryEvent()
        {
            var handler = new SummaryEventHandler(_db);
            handler.Handle(new TestSummaryEvent
            {
                TestId = "2",
                TestCase = "testcase 2",
                Iterations = 1,
                AverageMilliseconds = 1,
                TotalMilliseconds = 1,
                Throughput = 1.1,
                Slowest = 1,
                Fastest = 1
            });

            var se = _db.Query(nameof(SummaryEvents))
                .Select()
                .Where("TestId", "2")
                .First<SummaryEvents>();

            se.TestCase.Should().Be("testcase 2");
            se.Type.Should().Be("TestSummary");
        }
    }

    public class SummaryEvents
    {
        public string TestId { get; set; }

        public string TestCase { get; set; }

        public string Type { get; set; }
    }
}
