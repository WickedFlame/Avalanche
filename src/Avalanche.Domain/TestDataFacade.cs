using Avalanche.Domain.Models;
using Avalanche.ReadModel.Models;
using Avalanche.ReadModel.Queries;
using Avalanche.ReadModel.QueryHandlers;
using Avalanche.Runner;
using Avalanche.WriteModel.Events;

namespace Avalanche.Domain
{
    public class TestDataFacade
    {
        private readonly TestRunQueryHandler _queryHandler;

        public TestDataFacade(TestRunQueryHandler queryHandler)
        {
            _queryHandler = queryHandler;
        }

        public IEnumerable<ChartData> GetChartData(string testId, string testName)
        {
            var data = _queryHandler.Get(new GetChartData { TestId = testId, TestName = testName });

            return data
                .GroupBy(c => c.ThreadId)
                .Select(g => new ChartData
                {
                    Name = g.Key.ToString(),
                    Data = g.Select(e => new ChartDataRow
                        {
                            Time = e.Time.ToString("o"),
                            Value = e.Throughput.ToString()
                        })
                });
            ;
        }

        public IEnumerable<ChartDataRow> GetRampupData(string testId, string testName)
        {
            var data = _queryHandler.Get(new GetRampupData { TestId = testId, TestName = testName });

            var lst = new List<ChartDataRow>();


            if (data == null)
            {
                return Enumerable.Empty<ChartDataRow>();
            }

            var cnt = 0;
            foreach (var item in data.Where(v => v.Value > 0).OrderBy(d => d.Time))
            {
                cnt +=item.Value;
                lst.Add(new ChartDataRow { Time = item.Time.ToString("o"), Value = cnt.ToString() });
            }

            var firstDown = data.Where(v => v.Value < 0).OrderBy(d => d.Time).FirstOrDefault();
            if (firstDown != null)
            {
                lst.Add(new ChartDataRow { Time = firstDown.Time.ToString("o"), Value = cnt.ToString() });
            }

            foreach (var item in data.Where(v => v.Value < 0).OrderBy(d => d.Time))
            {
                cnt += item.Value;
                lst.Add(new ChartDataRow { Time = item.Time.ToString("o"), Value = cnt.ToString() });
            }


            var test = _queryHandler.Get(new GetTestRun { TestId = testId });

            if (test != null && test.Status != TestRunStatus.Done)
            {
                lst.Add(new ChartDataRow { Time = DateTime.Now.ToString("o"), Value = cnt.ToString() });
            }

            return lst;

        }

        public IEnumerable<TestSummary> GetSummary(string testId, string testCase)
        {
            return _queryHandler.Get(new GetSummary { TestId = testId, TestCase = testCase });
        }
    }
}
