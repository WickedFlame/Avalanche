using Avalanche.WriteModel.Commands;
using Avalanche.WriteModel.Events;
using Broadcast;

namespace Avalanche.WriteModel.CommandHandlers
{
    public class TestResultCommandHandler : CommandHandler<TestResultCommand>
    {
        private readonly IEventStore _store;
        private readonly IMessageBus _messageBus;

        public TestResultCommandHandler(IEventStore store, IMessageBus messageBus)
        {
            _store = store;
            _messageBus = messageBus;
        }

        public override void Handle(TestResultCommand cmd)
        {
            foreach(var result in cmd.Summary)
            {
                var @event = new ThreadSummaryEvent
                {
                    ThreadNumber = result.ThreadNumber,
                    Iterations = result.Iterations,
                    AverageMilliseconds = result.AverageMilliseconds,
                    AverageTicks = result.AverageTicks,
                    TotalTime = result.TotalTime,
                    Fastest = result.Fastest,
                    Slowest = result.Slowest,
                    Increase = result.Increase,
                    InitialSize = result.InitialSize,
                    EndSize = result.EndSize,
                };

                _store.Add(cmd.TestId, @event.GetType().AssemblyQualifiedName, DateTime.Now, @event);
                _messageBus.Send(@event);
            }


            var sumary = new TestSummaryEvent
            {
                Threads = cmd.Summary.Count(),
                Iterations = cmd.Iterations,
                AverageMilliseconds = cmd.AverageMilliseconds,
                AverageTicks = cmd.AverageTicks,
                TotalTime = cmd.TotalTime,
                Fastest = cmd.Fastest,
                Slowest = cmd.Slowest,
                Increase = cmd.Increase,
                InitialSize = cmd.InitialSize,
                EndSize = cmd.EndSize,
            };

            _store.Add(cmd.TestId, sumary.GetType().AssemblyQualifiedName, DateTime.Now, sumary);
            _messageBus.Send(sumary);
        }
    }
}
