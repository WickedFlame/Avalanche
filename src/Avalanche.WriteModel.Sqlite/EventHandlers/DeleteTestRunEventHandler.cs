using Avalanche.WriteModel.Events;
using Avalanche.WriteModel.Sqlite.DTO;
using SqlKata.Execution;
using System.Data.SQLite;

namespace Avalanche.WriteModel.Sqlite.EventHandlers
{
    public class DeleteTestRunEventHandler :
        IEventHandler<DeleteTestRunEvent>
    {
        private readonly QueryFactory _db;

        public DeleteTestRunEventHandler(QueryFactory db)
        {
            _db = db;
        }


        public void Handle(DeleteTestRunEvent @event)
        {
            _db.Query(nameof(TestRun))
                .Where(new
                {
                    TestId = "del1"
                })
                .Delete();

            _db.Query(nameof(SummaryEvents))
                .Where(new
                {
                    TestId = "del1"
                })
                .Delete();

            _db.Query(nameof(IterationEvents))
                .Where(new
                {
                    TestId = "del1"
                })
                .Delete();

            _db.Query(nameof(TestRunDetail))
                .Where(new
                {
                    TestId = "del1"
                })
                .Delete();

            _db.Query(nameof(RampupEvents))
                .Where(new
                {
                    TestId = "del1"
                })
                .Delete();
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
                _db.Dispose();
            }
        }
    }
}
