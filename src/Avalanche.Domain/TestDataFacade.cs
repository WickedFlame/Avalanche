using Avalanche.Domain.Models;
using Avalanche.ReadModel.Models;
using Avalanche.ReadModel.Queries;
using Avalanche.ReadModel.QueryHandlers;
using Avalanche.Runner;

namespace Avalanche.Domain
{
    public class TestDataFacade
    {
        public IEnumerable<ChartDataRow> GetRampupData(string testId)
        {
            var handler = new TestRunQueryHandler();
            var data = handler.Get(new GetRampupData { TestId = testId });

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


            var test = handler.Get(new GetTestRun { TestId = testId });

            if (test != null && test.Status != TestRunStatus.Done)
            {
                lst.Add(new ChartDataRow { Time = DateTime.Now.ToString("o"), Value = cnt.ToString() });
            }

            return lst;

        }

        public IEnumerable<TestSummary> GetSummary(string testId)
        {
            var handler = new TestRunQueryHandler();
            return handler.Get(new GetSummary { TestId = testId });
        }
    }
}
