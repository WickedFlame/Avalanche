using Avalanche.WriteModel.Commands;
using Broadcast;

namespace Avalanche.WriteModel.CommandHandlers
{
    public class IterationCommandHandler : CommandHandler<IterationCommand>
    {
        private readonly IEventBus _eventBus;
        private readonly Dictionary<string, IterationElementsContainer> _events = new();

        public IterationCommandHandler(IEventBus eventBus)
        {
            _eventBus = eventBus;
        }

        public override void Handle(IterationCommand cmd)
        {
            if(cmd.IsWarmup)
            {
                return;
            }

            var key = $"{cmd.TestId}_{cmd.Name}_{cmd.Thread}";
            if (!_events.ContainsKey(key))
            {
                _events.Add(key, new IterationElementsContainer());
            }

            var lst = _events[key];
            lst.Add(cmd);

            // only write to db if the last update was more than 2 seconds ago
            if (!lst.IsCheckValid())
            {
                return;
            }

            var @event = new Events.IterationLogEvent
            {
                TestId = cmd.TestId,
                Name = cmd.Name,
                Thread = cmd.Thread,
                Time = cmd.Time,
                Througput = lst.GetThroughput(),
                Iterations = lst.Count(),
                AverageMilliseconds = lst.GetAverageMilliseconds()
            };

            _eventBus.Publish(cmd.TestId, cmd.Time, @event);
        }
    }
}
