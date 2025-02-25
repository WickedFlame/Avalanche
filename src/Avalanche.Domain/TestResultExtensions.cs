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

            return logger.GetCollections()
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

        public static IEnumerable<ChartDataRow> GetRampupData(this Logger logger)
        {
            if (logger == null)
            {
                return Enumerable.Empty<ChartDataRow>();
            }

            var startData = logger.GetCollections()
                .SelectMany(g => g.Events.OfType<StartupLogEvent>())
                .Where(g => !g.IsWarmup)
                .ToList();
            var endData = logger.GetCollections()
                .SelectMany(g=>g.Events.OfType<EndLogEvent>())
                .Where(g => !g.IsWarmup)
                .ToList();

            var data = new List<ChartDataRow>();
            var cnt = 0;
            foreach(var item in  startData.OrderBy(d => d.Time))
            {
                cnt++;
                data.Add(new ChartDataRow { Time = item.Time.ToString("o"), Value = cnt.ToString() });
            }

            foreach (var item in endData.OrderBy(d => d.Time))
            {
                data.Add(new ChartDataRow { Time = item.Time.ToString("o"), Value = cnt.ToString() });
                cnt--;
            }

            if(!endData.Any() && logger.IsRunning)
            {
                data.Add(new ChartDataRow { Time = DateTime.Now.ToString("o"), Value = cnt.ToString() });
            }

            return data;
        }
    }
}
