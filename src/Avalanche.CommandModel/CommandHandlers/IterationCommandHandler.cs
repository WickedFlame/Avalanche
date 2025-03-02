using Avalanche.CommandModel;
using Avalanche.CommandModel.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace Avalanche.CommandModel.CommandHandlers
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

        public void Execute<T>(string id, T command) where T : class, ICommand
        {
            var cmd = command as IterationCommand;
            if (cmd == null)
            {
                return;
            }

            _store.Add(id, typeof(T).AssemblyQualifiedName, cmd);

            //TODO: create the readmodel
            var entry = new Events.IterationLogEvent
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

            //TODO: remove this to the readmodel
            _collection.Add(entry);
        }
    }
}
