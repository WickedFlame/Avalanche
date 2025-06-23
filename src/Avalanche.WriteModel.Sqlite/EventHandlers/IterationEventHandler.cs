using Avalanche.DataSource;
using Avalanche.WriteModel.Events;
using Avalanche.WriteModel.Sqlite.DTO;
using SqlKata.Execution;

namespace Avalanche.WriteModel.Sqlite.EventHandlers
{
    public class IterationEventHandler :
        IEventHandler<IterationLogEvent>,
        IEventHandler<IterationErrorEvent>
    {
        private readonly IProjectionConnectionBuilder _builder;

        public IterationEventHandler(IProjectionConnectionBuilder builder)
        {
            _builder = builder;
        }

        public void Handle(IterationLogEvent @event)
        {
            var db = _builder.Build();
            db.Query(nameof(IterationEvents))
                .Insert(new
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

            var trd = db.Query(nameof(TestRunDetail))
                .Select()
                .Where(new
                {
                    TestId = @event.TestId,
                    TestCase = @event.TestName,
                    ThreadId = @event.Thread
                })
                .Get();

            var query = db.Query(nameof(TestRunDetail));

            if (trd.Any())
            {
                query.AsUpdate(new
                    {
                        Throughput = @event.Throughput,
                        Iterations = @event.Iterations
                    })
                    .Where(new
                    {
                        TestId = @event.TestId,
                        TestCase = @event.TestName,
                        ThreadId = @event.Thread
                    });
            }
            else
            {
                query.AsInsert(new
                {
                    TestId = @event.TestId,
                    TestCase = @event.TestName,
                    ThreadId = @event.Thread,
                    Throughput = @event.Throughput,
                    Iterations = @event.Iterations
                });
            }

            db.Execute(query);
        }

        public void Handle(IterationErrorEvent @event)
        {
            var db  = _builder.Build();
            db.Query(nameof(IterationEvents))
                .Insert(new
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
