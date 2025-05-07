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

        public double AverageMilliseconds { get; set; }

        public double TotalMilliseconds { get; set; }

        public double Throughput { get; set; }
    }
}
