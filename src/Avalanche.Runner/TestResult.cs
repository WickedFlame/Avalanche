using MeasureMap;
using System.Collections;

namespace Avalanche.Runner
{
    public class TestResult : IProfilerResult
    {
        private readonly IProfilerResult _result;

        public TestResult(IProfilerResult result)
        {
            _result = result;
        }

        public string Name { get; set; }

        public int ThreadId => _result.ThreadId;

        public int ThreadNumber => _result.ThreadNumber;

        public IEnumerable<IIterationResult> Iterations => _result.Iterations;

        public IDictionary<string, object> ResultValues => _result.ResultValues;

        public double AverageMilliseconds => _result.AverageMilliseconds;

        public long AverageTicks => _result.AverageTicks;

        public TimeSpan AverageTime => _result.AverageTime;

        public TimeSpan TotalTime => _result.TotalTime;

        public IIterationResult Fastest => _result.Fastest;

        public IIterationResult Slowest => _result.Slowest;

        public long Increase => _result.Increase;

        public long InitialSize => _result.InitialSize;

        public long EndSize => _result.EndSize;

        public IEnumerator<IResult> GetEnumerator()
        {
            return _result.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _result.GetEnumerator();
        }
    }
}
