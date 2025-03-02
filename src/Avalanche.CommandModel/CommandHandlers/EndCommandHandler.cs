using Avalanche.CommandModel;
using Avalanche.CommandModel.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Avalanche.CommandModel.CommandHandlers
{
    public class EndCommandHandler : ICommandHandler
    {
        private readonly IEventStore _store;
        private readonly TestResultsCollection _collection;

        public EndCommandHandler(IEventStore store, TestResultsCollection collection)
        {
            _store = store;
            _collection = collection;
        }

        public void Execute<T>(string id, T command) where T : class, ICommand
        {
            var cmd = command as EndCommand;
            if (cmd == null)
            {
                return;
            }

            _store.Add(id, typeof(T).AssemblyQualifiedName, cmd);

            //TODO: create the readmodel
            var entry = new Events.EndLogEvent
            {
                Category = cmd.Category,
                Module = cmd.Module,
                Name = cmd.Name,
                Message = cmd.Message,
                Thread = cmd.Thread,
                IsWarmup = cmd.IsWarmup
            };

            //TODO: remove this to the readmodel
            _collection.Add(entry);
        }
    }
}
