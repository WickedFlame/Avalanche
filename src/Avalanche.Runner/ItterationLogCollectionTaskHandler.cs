using Avalanche.WriteModel;
using Avalanche.WriteModel.Commands;
using Broadcast;
using MeasureMap;

namespace Avalanche.Runner
{
    public class ItterationLogCollectionTaskHandler : TaskHandler
    {
        private readonly string _name;
        private readonly string _testId;

        public ItterationLogCollectionTaskHandler(string name, string testId)
        {
            _name = name;
            _testId = testId;
        }

        public override IIterationResult Run(IExecutionContext context)
        {
            var log = context.Get<IDispatcher<ICommand>>(nameof(IDispatcher<ICommand>));

            var result = base.Run(context);

            var metric = new IterationCommand
            {
                TestId = _testId,
                Category = "console",
                Module = "Measure",
                Name = _name,
                Message = $"Run number {result.Iteration} on Thread {result.ThreadNumber}",
                RunNumber = result.Iteration,
                Thread = result.ThreadNumber,
                IsWarmup = context.Settings.IsWarmup,
                TotalMilliseconds = result.Duration.TotalMilliseconds,
                Time = result.TimeStamp
            };

            log.SendAsync(metric);

            return result;
        }
    }
}
