using Avalanche.WriteModel.Events;
using Avalanche.WriteModel.Sqlite.DTO;
using SqlKata.Execution;
using System.Data.SQLite;

namespace Avalanche.WriteModel.Sqlite.EventHandlers
{
    public class TestRunEventHandler :
        IEventHandler<StartTestEvent>,
        IEventHandler<EndTestEvent>
    {
        private readonly SQLiteConnection _connection;
        private QueryFactory _db;

        public TestRunEventHandler(QueryFactory db)
        {
            _connection = new SQLiteConnection(Constants.ReadModelDatabase);
            _connection.Open();

            _db = db;
        }

        public void Handle(StartTestEvent evnt)
        {
            _db.Query(nameof(TestRun))
                .Insert(new
                {
                    TestId = evnt.TestId,
                    Scenario = evnt.Scenario,
                    StartTime = evnt.StartTime,
                    Status = evnt.Status
                });
        }

        public void Handle(EndTestEvent evnt)
        {
            _db.Query(nameof(TestRun))
                .Where(new
                {
                    TestId = evnt.TestId
                })
                .Update(new
                {
                    TestId = evnt.TestId,
                    EndTime = evnt.EndTime,
                    Status = evnt.Status
                });
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // do stuf here
                _connection.Close();
                _connection.Dispose();
            }
        }
    }
}
