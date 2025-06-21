using Avalanche.WriteModel.Events;
using Avalanche.WriteModel.Sqlite.DTO;
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
    public class TestRunEventHandlerTests
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
        public void TestRunEventHandler_StartTestEvent()
        {
            var handler = new TestRunEventHandler(_db);
            handler.Handle(new StartTestEvent
            {
                TestId = "1",
                Scenario = "Scenario 1",
                StartTime = DateTime.Now,
                Status = "Started"
            });

            var se = _db.Query(nameof(TestRun))
                .Select()
                .Where("TestId", "1")
                .First<TestRun>();

            se.Scenario.Should().Be("Scenario 1");
            se.Status.Should().Be("Started");
            se.EndTime.Should().Be(DateTime.MinValue);
        }

        [Test]
        public void TestRunEventHandler_EndTestEvent()
        {
            _db.Query(nameof(TestRun)).Insert(new
            {
                TestId = "2",
                Scenario = "Scenario 2",
                StartTime = DateTime.Now,
                Status = "Started"
            });

            var handler = new TestRunEventHandler(_db);
            handler.Handle(new EndTestEvent
            {
                TestId = "2",
                EndTime = DateTime.Now,
                Status = "Ended"
            });

            var se = _db.Query(nameof(TestRun))
                .Select()
                .Where("TestId", "2")
                .First<TestRun>();

            se.Status.Should().Be("Ended");
            se.EndTime.Should().BeAfter(DateTime.Now.AddMinutes(-1));
        }
    }
}
