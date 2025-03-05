using Avalanche.WriteModel;
using Avalanche.WriteModel.Events;
using Avalanche.Domain.Models;
using Avalanche.Runner;
using Avalanche.Runner.Logging;

namespace Avalanche
{
    public static class TestResultExtensions
    {
        public static IEnumerable<TestResultsCollection> GetCollections(this TestRunData data)
        {
            return data.Collections;
        }

        public static IEnumerable<IterationLogEvent> GetItterationEvents(this TestRunData data)
        {
            return data.GetCollections().SelectMany(x => x.Events.OfType<IterationLogEvent>());
        }

        public static IEnumerable<ChartData> GetChartData(this TestRunData data)
        {
            if(data== null)
            {
                return Enumerable.Empty<ChartData>();
            }

            return data.GetCollections()
                .Where(c=> !c.IsWarmup)
                .Select(g => new ChartData
                {
                    Name = g.ThreadId,
                    Data = g.Events.OfType<IterationLogEvent>()
                        .Where(d => !d.IsWarmup)
                        .Select(e => new ChartDataRow
                        {
                            Time = e.Time.ToString("o"),
                            Value = e.TotalMilliseconds.ToString()
                        })
                });
        }
    }
}
