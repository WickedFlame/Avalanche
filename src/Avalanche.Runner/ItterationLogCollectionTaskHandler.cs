using Avalanche.WriteModel;
using Avalanche.WriteModel.Commands;
using Broadcast;
using MeasureMap;

namespace Avalanche.Runner
{
    public class ItterationLogCollectionTaskHandler : TaskHandler
    {
        private readonly string _testCase;
        private readonly string _testId;

        public ItterationLogCollectionTaskHandler(string testCase, string testId)
        {
            _testCase = testCase;
            _testId = testId;
        }

        public override IIterationResult Run(IExecutionContext context)
        {
            var log = context.Get<IDispatcher<ICommand>>(nameof(IDispatcher<ICommand>));

            var result = base.Run(context);

            var metric = new IterationCommand
            {
                TestId = _testId,
                TestCase = _testCase,
                Message = $"Run number {result.Iteration} on Thread {result.ThreadId}",
                RunNumber = result.Iteration,
                ThreadId = result.ThreadId,
                IsWarmup = context.Settings.IsWarmup,
                TotalMilliseconds = result.Duration.TotalMilliseconds,
                Time = result.TimeStamp,
                ContentLength = context.Get<long?>("ContentLength")
            };

            log.SendAsync(metric);

            return result;
        }
    }
}
