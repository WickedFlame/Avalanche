using Avalanche.DataSource;
using Avalanche.DataSource.DTO;
using Avalanche.WriteModel.Events;
using Broadcast;
using SqlKata.Execution;

namespace Avalanche.WriteModel.Sql.EventHandlers
{
    public class SettingsEventHandler :
        IEventHandler<AddApiKeyEvent>,
        IEventHandler<DeleteApiKeyEvent>
    {
        private readonly IProjectionConnectionBuilder _builder;

        public SettingsEventHandler(IProjectionConnectionBuilder builder)
        {
            _builder = builder;
        }

        public void Handle(AddApiKeyEvent @event)
        {
            var db = _builder.Build();
            db.Query(nameof(ApiKeys))
                .Insert(new
                {
                    @event.Name,
                    @event.Value,
                    @event.Created,
                    @event.Expires
                });
        }

        public void Handle(DeleteApiKeyEvent @event)
        {
            var db = _builder.Build();
            db.Query(nameof(ApiKeys))
                .Where(new
                {
                    @event.Name
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
