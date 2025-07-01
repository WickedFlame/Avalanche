using Avalanche.DataSource.DTO;
using Avalanche.DataSource.Sqlite;
using Avalanche.WriteModel.Sql.EventHandlers;
using SqlKata.Compilers;
using SqlKata.Execution;
using System.Data.SQLite;

namespace Avalanche.WriteModel.Sql.IntegrationTests.EventHandlers
{
    public class IterationEventHandlerTests
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
        public void IterationEventHandler_IterationLogEvent()
        {
            var handler = new IterationEventHandler(new ProjectionConnectionBuilder());
            handler.Handle(new Events.IterationLogEvent
            {
                TestId = "1",
                Time = DateTime.Now,
                Thread = 1,
                TestName = "First",
                AverageMilliseconds = 123,
                IsWarmup = false,
                Throughput = 2.3,
                Iterations = 21
            });

            var ie = _db.Query(nameof(IterationEvents))
                .Select("TestId")
                .Where("TestId", "1")
                .First<IterationEvents>();

            ie.TestId.Should().Be("1");

            var trd = _db.Query(nameof(TestRunDetail))
                .Select()
                .Where("TestId", "1")
                .First<TestRunDetail>();

            trd.Throughput.Should().Be(2.3);
        }

        [Test]
        public void IterationEventHandler_IterationLogEvent_Update()
        {
            var handler = new IterationEventHandler(new ProjectionConnectionBuilder());
            handler.Handle(new Events.IterationLogEvent
            {
                TestId = "2",
                Time = DateTime.Now,
                Thread = 1,
                TestName = "First",
                AverageMilliseconds = 123,
                IsWarmup = false,
                Throughput = 2.3,
                Iterations = 21
            });
            handler.Handle(new Events.IterationLogEvent
            {
                TestId = "2",
                Time = DateTime.Now,
                Thread = 1,
                TestName = "First",
                AverageMilliseconds = 123,
                IsWarmup = false,
                Throughput = 1.2,
                Iterations = 50
            });
            
            var trd = _db.Query(nameof(TestRunDetail))
                .Select()
                .Get<TestRunDetail>();

            trd.Single(t => t.Throughput == 1.2).TestId.Should().Be("2");
        }

        [Test]
        public void IterationEventHandler_IterationErrorEvent()
        {
            var handler = new IterationEventHandler(new ProjectionConnectionBuilder());
            handler.Handle(new Events.IterationErrorEvent
            {
                TestId = "3",
                Time = DateTime.Now,
                Thread = 1,
                TestName = "First",
                Message = "the message",
                StatusCode = "503"
            });

            var ie = _db.Query(nameof(IterationEvents))
                .Select()
                .Where("TestId", "3")
                .First<IterationEvents>();

            ie.Error.Should().Be(true);
            ie.StatusCode.Should().Be("503");
            ie.Message.Should().Be("the message");
        }
    }
}
