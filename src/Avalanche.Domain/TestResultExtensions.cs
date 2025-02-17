using Avalanche.Domain.Models;
using Avalanche.Runner.Logging;

namespace Avalanche
{
    public static class TestResultExtensions
    {
        public static IEnumerable<IterationLogEvent> GetItterationEvents(this Logger logger)
        {
            return logger.GetCollections().SelectMany(x => x.Events.OfType<IterationLogEvent>());
        }

        public static IEnumerable<ChartData> GetChartData(this Logger logger)
        {
            if(logger== null)
            {
                return Enumerable.Empty<ChartData>();
            }

            return logger.GetCollections().Select(g => new ChartData
            {
                Name = g.ThreadId,
                Data = g.Events.OfType<IterationLogEvent>().Select(e => new ChartDataRow
                {
                    Time = e.Time.ToString("o"),
                    Value = e.TotalMilliseconds.ToString()
                })
            });
        }
    }
}
