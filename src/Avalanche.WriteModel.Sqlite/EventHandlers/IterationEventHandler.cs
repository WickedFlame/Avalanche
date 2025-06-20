using Avalanche.WriteModel.Events;
using SqlKata.Execution;

namespace Avalanche.WriteModel.Sqlite.EventHandlers
{
    public class IterationEventHandler :
        IEventHandler<IterationLogEvent>,
        IEventHandler<IterationErrorEvent>
    {
        private readonly QueryFactory _db;

        public IterationEventHandler(QueryFactory db)
        {
            _db = db;
        }

        public void Handle(IterationLogEvent @event)
        {
            _db.Query("IterationEvents").Insert(new
            {
                Id = Guid.NewGuid().ToString(),
                TestId = @event.TestId,
                Time = @event.Time,
                ThreadId = @event.Thread,
                TestName = @event.TestName,
                AverageMilliseconds = @event.AverageMilliseconds,
                Throughput = @event.Throughput,
                IsWarmup = @event.IsWarmup
            });

            var trd = _db.Query("TestRunDetail")
                .Select()
                .Where(new
                {
                    @event.TestId,
                    TestCase = @event.TestName,
                    ThreadId = @event.Thread
                })
                .Get();

            var query = _db.Query("TestRunDetail");
            var detail = new
            {
                @event.TestId,
                TestCase = @event.TestName,
                ThreadId = @event.Thread,
                Throughput = @event.Throughput,
                Iterations = @event.Iterations
            };

            if (trd.Any())
            {
                query.AsUpdate(detail).Where(new
                {
                    TestId = @event.TestId,
                    TestCase = @event.TestName,
                    ThreadId = @event.Thread
                });
            }
            else
            {
                query.AsInsert(detail);
            }

            _db.Execute(query);
        }

        public void Handle(IterationErrorEvent @event)
        {
            _db.Query("IterationEvents").Insert(new
            {
                Id = Guid.NewGuid().ToString(),
                TestId = @event.TestId,
                Time = @event.Time,
                ThreadId = @event.Thread,
                TestName = @event.TestName,
                Message = @event.Message,
                StatusCode = @event.StatusCode,
                Error = true
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
                // do stuf here;
            }
        }
    }
}
