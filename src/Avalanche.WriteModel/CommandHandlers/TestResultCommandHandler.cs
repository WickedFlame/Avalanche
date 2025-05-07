using Avalanche.WriteModel.Commands;
using Avalanche.WriteModel.Events;
using Broadcast;

namespace Avalanche.WriteModel.CommandHandlers
{
    public class TestResultCommandHandler : CommandHandler<TestResultCommand>
    {
        private readonly IEventBus _eventBus;

        public TestResultCommandHandler(IEventBus eventBus)
        {
            _eventBus = eventBus;
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
                    AverageMilliseconds = result.AverageMilliseconds,
                    TotalMilliseconds = result.TotalMilliseconds,
                    Throughput = result.Throughput
                };

                _eventBus.Publish(cmd.TestId, DateTime.Now, @event);
            }

            var sumary = new TestSummaryEvent
            {
                TestId = cmd.TestId,
                TestCase = cmd.TestCase,
                Threads = cmd.Summary.Count(),
                Iterations = cmd.Iterations,
                AverageMilliseconds = cmd.AverageMilliseconds,
                TotalMilliseconds = cmd.TotalMilliseconds,
                Throughput = cmd.Throughput
            };

            _eventBus.Publish(cmd.TestId, DateTime.Now, sumary);
        }
    }
}
