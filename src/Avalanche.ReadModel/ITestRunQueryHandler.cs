using Avalanche.ReadModel.Models;
using Avalanche.ReadModel.Queries;

namespace Avalanche.ReadModel
{
    public interface ITestRunQueryHandler :
        IQueryHandler<IEnumerable<TestRun>, GetTestsQuery>,
        IQueryHandler<IEnumerable<TestStatistic>, GetTestsStatisticsQuery>,
        IQueryHandler<TestRun, GetLastTestQuery>,
        IQueryHandler<TestRun, GetTestRun>,
        IQueryHandler<IEnumerable<RampupData>, GetRampupData>,
        IQueryHandler<IEnumerable<TestSummary>, GetSummary>
    {
    }
}
