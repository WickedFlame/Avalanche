using Avalanche.CommandModel.Commands;

namespace Avalanche.CommandModel.CommandHandlers
{
    public class EndTestCommandHandler : ICommandHandler
    {
        private readonly IEventStore _store;

        public EndTestCommandHandler(IEventStore store)
        {
            _store = store;
        }

        public void Handle(ICommand command)
        {
            var cmd = command as EndTestCommand;
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
