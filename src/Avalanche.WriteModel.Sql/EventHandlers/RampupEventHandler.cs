using Avalanche.DataSource;
using Avalanche.DataSource.DTO;
using Avalanche.WriteModel.Events;
using SqlKata.Execution;

namespace Avalanche.WriteModel.Sql.EventHandlers
{
    public class RampupEventHandler :
        IEventHandler<RampupEvent>,
        IEventHandler<RampdownEvent>
    {
        private readonly IProjectionConnectionBuilder _builder;

        public RampupEventHandler(IProjectionConnectionBuilder builder)
        {
            _builder = builder;
        }

        public void Handle(RampupEvent @event)
        {
            if (@event.IsWarmup)
            {
                return;
            }

            var db = _builder.Build();
            db.Query(nameof(RampupEvents))
                .Insert(new
                {
                    Id = Guid.NewGuid().ToString(),
                    TestId = @event.TestId,
                    Time = DateTime.Now,
                    TestCase = @event.TestCase,
                    Value = 1
                });
        }

        public void Handle(RampdownEvent @event)
        {
            if (@event.IsWarmup)
            {
                return;
            }

            var db = _builder.Build();
            db.Query(nameof(RampupEvents))
                .Insert(new
                {
                    Id = Guid.NewGuid().ToString(),
                    TestId = @event.TestId,
                    Time = DateTime.Now,
                    TestCase = @event.TestCase,
                    ThreadId = @event.ThreadId,
                    Value = -1
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
            }
        }
    }
}
