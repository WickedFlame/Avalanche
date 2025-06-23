using Avalanche.DataSource;
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
        private readonly IProjectionConnectionBuilder _builder;

        public TestRunEventHandler(IProjectionConnectionBuilder builder)
        {
            _builder = builder;
        }

        public void Handle(StartTestEvent evnt)
        {
            var db = _builder.Build();
            db.Query(nameof(TestRun))
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
            var db = _builder.Build();
            db.Query(nameof(TestRun))
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
            }
        }
    }
}
