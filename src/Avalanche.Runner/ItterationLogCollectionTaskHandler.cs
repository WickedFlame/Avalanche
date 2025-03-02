using Avalanche.CommandModel;
using Avalanche.CommandModel.Commands;
using Avalanche.Runner.Logging;
using MeasureMap;

namespace Avalanche.Runner
{
    public class ItterationLogCollectionTaskHandler : TaskHandler
    {
        private readonly string _name;

        public ItterationLogCollectionTaskHandler(string name)
        {
            _name = name;
        }

        public override IIterationResult Run(IExecutionContext context)
        {
            var log = context.Get<ICommandDispatcher>(nameof(ICommandDispatcher));

            var result = base.Run(context);

            var metric = new IterationCommand
            {
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

            log.Add(metric);

            return result;
        }
    }
}
