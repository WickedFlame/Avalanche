namespace Avalanche.WriteModel.Commands
{
    public class TestResultCommand : ThreadSummary, ICommand
    {
        public string TestId {  get; set; }

        /// <summary>
        /// Name of the TestCase
        /// </summary>
        public string TestCase { get; set; }


        public IEnumerable<ThreadSummary> Summary { get; set; }
    }

    public class ThreadSummary
    {
        public int ThreadNumber { get; set; }

        public int Iterations { get; set; }

        public  long AverageTicks { get; set; }

        public long TotalTime { get; set; }

        public long Fastest { get; set; }

        public long Slowest { get; set; }

        public long Increase { get; set; }

        public long InitialSize { get; set; }

        public long EndSize { get; set; }
    }
}
