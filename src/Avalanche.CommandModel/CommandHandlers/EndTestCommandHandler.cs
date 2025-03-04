using Avalanche.CommandModel.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace Avalanche.CommandModel.CommandHandlers
{
    public class EndTestCommandHandler : ICommandHandler
    {
        private readonly IEventStore _store;
        private readonly TestResultsCollection _collection;

        public EndTestCommandHandler(IEventStore store, TestResultsCollection collection)
        {
            _store = store;
            _collection = collection;
        }

        public void Execute<T>(string testId, T command) where T : class, ICommand
        {
            var cmd = command as EndTestCommand;
            if (cmd == null)
            {
                return;
            }

            //TODO: create the readmodel
            //var @event = new Events.EndTestEvent
            //{
            //    TestId = cmd.TestId,
            //    TestName = cmd.TestName,
            //    StartTime = cmd.StartTime,
            //    Status = cmd.Status
            //};

            //_store.Add(testId, typeof(T).AssemblyQualifiedName, cmd.StartTime, @event);

            //TODO: remove this to the readmodel
            //_collection.Add(@event);
        }
    }
}
