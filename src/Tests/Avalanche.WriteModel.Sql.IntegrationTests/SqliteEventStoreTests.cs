using Avalanche.DataSource.Sqlite;
using Broadcast;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Polaroider;
using SqlKata.Compilers;
using SqlKata.Execution;
using System.Data.SQLite;

namespace Avalanche.WriteModel.Sql.IntegrationTests
{
    public class SqliteEventStoreTests
    {
        private QueryFactory _db;

        [SetUp]
        public void Setup()
        {
            var connection = new SQLiteConnection($"Data Source=data/{Constants.EventStore}.db");
            var compiler = new SqliteCompiler();
            _db = new QueryFactory(connection, compiler);
        }

        [TearDown]
        public void Teardown()
        {
            _db.Dispose();
        }

        [Test]
        public void SqliteEventStore_Add()
        {
            var store = new SqlEventStore(new EventStoreConnectionBuilder(Mock.Of<IConfiguration>()), Mock.Of<ILogger<SqlEventStore>>());

            store.Add("1", DateTime.Now, new TestEvent { Id = 1 });

            var events = _db.Query(nameof(Avalanche.DataSource.DTO.Events))
                .Where(new
                {
                    TestId = "1"
                })
                .Select()
                .Get<Avalanche.DataSource.DTO.Events>();

            events.Single().MatchSnapshot(SnapshotOptions.Create(o => o.MockDateTimes().MockGuids()));
        }

        public class TestEvent
        {
            public int Id { get; set; }
        }
    }
}
