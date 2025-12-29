using Avalanche.DataSource;
using Avalanche.DataSource.DTO;
using Avalanche.WriteModel.Events;
using Broadcast;
using SqlKata.Execution;

namespace Avalanche.WriteModel.Sql.EventHandlers
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
                    TestCase = @event.TestCase,
                    AverageMilliseconds = @event.AverageMilliseconds,
                    Throughput = @event.Throughput,
                    IsWarmup = @event.IsWarmup,
                    ContentLength = @event.ContentLength
                });

            var trd = db.Query(nameof(TestRunDetail))
                .Select()
                .Where(new
                {
                    TestId = @event.TestId,
                    TestCase = @event.TestCase,
                    ThreadId = @event.Thread
                })
                .Get();

            var query = db.Query(nameof(TestRunDetail));

            if (trd.Any())
            {
                query.AsUpdate(new
                    {
                        Throughput = @event.Throughput,
                        Iterations = @event.Iterations,
                        AverageMilliseconds = @event.AverageMilliseconds
                    })
                    .Where(new
                    {
                        TestId = @event.TestId,
                        TestCase = @event.TestCase,
                        ThreadId = @event.Thread
                    });
            }
            else
            {
                query.AsInsert(new
                {
                    TestId = @event.TestId,
                    TestCase = @event.TestCase,
                    ThreadId = @event.Thread,
                    Throughput = @event.Throughput,
                    Iterations = @event.Iterations,
                    AverageMilliseconds = @event.AverageMilliseconds
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
                    ThreadId = @event.ThreadId,
                    TestCase = @event.TestCase,
                    Message = @event.Message,
                    StatusCode = @event.StatusCode,
                    Error = true,
                    IsWarmup = @event.IsWarmup
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
