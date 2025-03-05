using Avalanche.WriteModel;
using Avalanche.WriteModel.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace Avalanche.WriteModel.CommandHandlers
{
    public class IterationCommandHandler : ICommandHandler
    {
        private readonly IEventStore _store;
        private readonly TestResultsCollection _collection;

        public IterationCommandHandler(IEventStore store, TestResultsCollection collection)
        {
            _store = store;
            _collection = collection;
        }

        public void Handle(ICommand command)
        {
            var cmd = command as IterationCommand;

            //TODO: create the readmodel
            var @event = new Events.IterationLogEvent
            {
                Category = cmd.Category,
                Module = cmd.Module,
                Name = cmd.Name,
                Message = cmd.Message,
                RunNumber = cmd.RunNumber,
                Thread = cmd.Thread,
                IsWarmup = cmd.IsWarmup,
                TotalMilliseconds = cmd.TotalMilliseconds,
                Time = cmd.Time
            };

            _store.Add(cmd.TestId, typeof(Events.IterationLogEvent).AssemblyQualifiedName, cmd.Time, @event);

            //TODO: remove this to the readmodel
            if (string.IsNullOrEmpty(_collection.ThreadId))
            {
                _collection.ThreadId = @event.Thread.ToString();
                _collection.IsWarmup = @event.IsWarmup;
            }

            _collection.Add(@event);
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
