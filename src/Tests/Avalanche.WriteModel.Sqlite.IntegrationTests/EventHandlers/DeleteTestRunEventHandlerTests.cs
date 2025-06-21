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
    public class DeleteTestRunEventHandlerTests
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
            _db.Query(nameof(TestRun))
                .Insert(new
                {
                    TestId = "del1"
                });
            _db.Query(nameof(SummaryEvents))
                .Insert(new
                {
                    TestId = "del1"
                });
            _db.Query(nameof(IterationEvents))
                .Insert(new
                {
                    TestId = "del1"
                });
            _db.Query(nameof(TestRunDetail))
                .Insert(new
                {
                    TestId = "del1"
                });
            _db.Query(nameof(RampupEvents))
                .Insert(new
                {
                    TestId = "del1"
                });
            

            var handler = new DeleteTestRunEventHandler(_db);
            handler.Handle(new DeleteTestRunEvent
            {
                TestId = "del1"
            });


            _db.Query(nameof(TestRun))
                .Select()
                .Where("TestId", "del1")
                .FirstOrDefault<TestRun>()
                .Should().BeNull();

            _db.Query(nameof(SummaryEvents))
                .Select()
                .Where("TestId", "del1")
                .FirstOrDefault<SummaryEvents>()
                .Should().BeNull();

            _db.Query(nameof(IterationEvents))
                .Select()
                .Where("TestId", "del1")
                .FirstOrDefault<IterationEvents>()
                .Should().BeNull();

            _db.Query(nameof(TestRunDetail))
                .Select()
                .Where("TestId", "del1")
                .FirstOrDefault<TestRunDetail>()
                .Should().BeNull();

            _db.Query(nameof(RampupEvents))
                .Select()
                .Where("TestId", "del1")
                .FirstOrDefault<RampupEvents>()
                .Should().BeNull();

        }
    }
}
