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
                    TestId = cmd.TestId,
                    TestCase = cmd.TestCase,
                    ThreadNumber = result.ThreadNumber,
                    Iterations = result.Iterations,
                    AverageTicks = result.AverageTicks,
                    TotalTime = result.TotalTime,
                    Fastest = result.Fastest,
                    Slowest = result.Slowest,
                    Increase = result.Increase,
                    InitialSize = result.InitialSize,
                    EndSize = result.EndSize,
                };

                _store.Add(cmd.TestId, DateTime.Now, @event);
                _messageBus.Send(@event);
            }

            var sumary = new TestSummaryEvent
            {
                TestId = cmd.TestId,
                TestCase = cmd.TestCase,
                Threads = cmd.Summary.Count(),
                Iterations = cmd.Iterations,
                AverageTicks = cmd.AverageTicks,
                TotalTime = cmd.TotalTime,
                Fastest = cmd.Fastest,
                Slowest = cmd.Slowest,
                Increase = cmd.Increase,
                InitialSize = cmd.InitialSize,
                EndSize = cmd.EndSize,
            };

            _store.Add(cmd.TestId, DateTime.Now, sumary);
            _messageBus.Send(sumary);
        }
    }
}
