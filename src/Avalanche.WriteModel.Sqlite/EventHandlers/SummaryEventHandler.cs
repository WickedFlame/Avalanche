using Avalanche.WriteModel.Events;
using Avalanche.WriteModel.Sqlite.DTO;
using SqlKata.Execution;

namespace Avalanche.WriteModel.Sqlite.EventHandlers
{
    public class SummaryEventHandler :
        IEventHandler<ThreadSummaryEvent>,
        IEventHandler<TestSummaryEvent>
    {
        private readonly QueryFactory _db;

        public SummaryEventHandler(QueryFactory db)
        {
            _db = db;
        }

        public void Handle(ThreadSummaryEvent @event)
        {
            _db.Query(nameof(SummaryEvents))
                .Insert(new
                {
                    Id = Guid.NewGuid().ToString(),
                    TestId = @event.TestId,
                    Time = DateTime.Now,
                    TestCase = @event.TestCase,
                    Type = "ThreadSummary",
                    ThreadNumber = @event.ThreadNumber,
                    Iterations = @event.Iterations,
                    AverageMilliseconds = @event.AverageMilliseconds,
                    TotalMilliseconds = @event.TotalMilliseconds,
                    Throughput = @event.Throughput
                });
        }

        public void Handle(TestSummaryEvent @event)
        {
            _db.Query(nameof(SummaryEvents))
                .Insert(new
                {
                    Id = Guid.NewGuid().ToString(),
                    TestId = @event.TestId,
                    Time = DateTime.Now,
                    TestCase = @event.TestCase,
                    Type = "TestSummary",
                    Iterations = @event.Iterations,
                    AverageMilliseconds = @event.AverageMilliseconds,
                    TotalMilliseconds = @event.TotalMilliseconds,
                    Throughput = @event.Throughput,
                    Slowest = @event.Slowest,
                    Fastest = @event.Fastest
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
                _db.Dispose();
            }
        }
    }
}
