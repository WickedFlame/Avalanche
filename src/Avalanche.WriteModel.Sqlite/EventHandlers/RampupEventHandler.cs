using Avalanche.WriteModel.Events;
using Avalanche.WriteModel.Sqlite.DTO;
using SqlKata.Execution;

namespace Avalanche.WriteModel.Sqlite.EventHandlers
{
    public class RampupEventHandler :
        IEventHandler<RampupEvent>,
        IEventHandler<RampdownEvent>
    {
        private readonly QueryFactory _db;

        public RampupEventHandler(QueryFactory db)
        {
            _db = db;
        }

        public void Handle(RampupEvent @event)
        {
            if (@event.IsWarmup)
            {
                return;
            }

            _db.Query(nameof(RampupEvents))
                .Insert(new
                {
                    Id = Guid.NewGuid().ToString(),
                    TestId = @event.TestId,
                    Time = DateTime.Now,
                    Name = @event.Name,
                    Value = 1
                });
        }

        public void Handle(RampdownEvent @event)
        {
            if (@event.IsWarmup)
            {
                return;
            }

            _db.Query(nameof(RampupEvents))
                .Insert(new
                {
                    Id = Guid.NewGuid().ToString(),
                    TestId = @event.TestId,
                    Time = DateTime.Now,
                    Name = @event.Name,
                    ThreadId = @event.Thread,
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
                _db.Dispose();
            }
        }
    }
}
