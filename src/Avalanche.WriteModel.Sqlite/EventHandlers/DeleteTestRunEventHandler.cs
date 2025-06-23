using Avalanche.DataSource;
using Avalanche.WriteModel.Events;
using Avalanche.WriteModel.Sqlite.DTO;
using SqlKata.Execution;
using System.Data.SQLite;

namespace Avalanche.WriteModel.Sqlite.EventHandlers
{
    public class DeleteTestRunEventHandler :
        IEventHandler<DeleteTestRunEvent>
    {
        private readonly IProjectionConnectionBuilder _builder;

        public DeleteTestRunEventHandler(IProjectionConnectionBuilder builder)
        {
            _builder = builder;
        }


        public void Handle(DeleteTestRunEvent @event)
        {
            var db = _builder.Build();
            db.Query(nameof(TestRun))
                .Where(new
                {
                    TestId = @event.TestId
                })
                .Delete();

            db.Query(nameof(SummaryEvents))
                .Where(new
                {
                    TestId = @event.TestId
                })
                .Delete();

            db.Query(nameof(IterationEvents))
                .Where(new
                {
                    TestId = @event.TestId
                })
                .Delete();

            db.Query(nameof(TestRunDetail))
                .Where(new
                {
                    TestId = @event.TestId
                })
                .Delete();

            db.Query(nameof(RampupEvents))
                .Where(new
                {
                    TestId = @event.TestId
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
            }
        }
    }
}
