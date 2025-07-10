using Avalanche.DataSource.DTO;
using Avalanche.DataSource.Sqlite;
using Avalanche.WriteModel.Events;
using Avalanche.WriteModel.Sql.EventHandlers;
using SqlKata.Compilers;
using SqlKata.Execution;
using System.Data.SQLite;

namespace Avalanche.WriteModel.Sql.IntegrationTests.EventHandlers
{
    public class RampupEventHandlerTests
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
        public void RampupEventHandler_RampupEvent()
        {
            var handler = new RampupEventHandler(new ProjectionConnectionBuilder());
            handler.Handle(new RampupEvent
            {
                TestId = "1",
                TestCase = "test 1",
                Time = DateTime.Now
            });

            var se = _db.Query(nameof(RampupEvents))
                .Select()
                .Where("TestId", "1")
                .First<RampupEvents>();

            se.TestCase.Should().Be("test 1");
            se.Time.Should().BeAfter(DateTime.Now.AddMinutes(-1));
            se.Value.Should().Be(1);
        }

        [Test]
        public void RampupEventHandler_RampupEvent_Warmup()
        {
            var handler = new RampupEventHandler(new ProjectionConnectionBuilder());
            handler.Handle(new RampupEvent
            {
                TestId = "2",
                TestCase = "test 2",
                Time = DateTime.Now,
                IsWarmup = true
            });

            var se = _db.Query(nameof(RampupEvents))
                .Select()
                .Where("TestId", "2")
                .FirstOrDefault<RampupEvents>();

            se.Should().BeNull();
        }





        [Test]
        public void RampupEventHandler_RampdownEvent()
        {
            var handler = new RampupEventHandler(new ProjectionConnectionBuilder());
            handler.Handle(new RampdownEvent
            {
                TestId = "3",
                TestCase = "test 3",
                Time = DateTime.Now,
                ThreadId = 1
            });

            var se = _db.Query(nameof(RampupEvents))
                .Select()
                .Where("TestId", "3")
                .First<RampupEvents>();

            se.TestCase.Should().Be("test 3");
            se.Time.Should().BeAfter(DateTime.Now.AddMinutes(-1));
            se.ThreadId.Should().Be(1);
            se.Value.Should().Be(-1);
        }

        [Test]
        public void RampupEventHandler_RampdownEvent_Warmup()
        {
            var handler = new RampupEventHandler(new ProjectionConnectionBuilder());
            handler.Handle(new RampdownEvent
            {
                TestId = "4",
                TestCase = "test 4",
                Time = DateTime.Now,
                ThreadId = 1,
                IsWarmup = true
            });

            var se = _db.Query(nameof(RampupEvents))
                .Select()
                .Where("TestId", "4")
                .FirstOrDefault<RampupEvents>();

            se.Should().BeNull();
        }
    }
}
