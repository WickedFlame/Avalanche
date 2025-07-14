
namespace Avalanche.WriteModel.Commands
{
    public class IterationCommand : ICommand
    {
        public IterationCommand()
        {
            Time = DateTime.Now;
        }

        public string TestId { get; set; }

        public DateTime Time { get; set; }

        public string TestCase { get; set; }

        public string Message { get; set; }

        public int RunNumber { get; set; }

        public int ThreadId { get; set; }

        public bool IsWarmup { get; set; }

        public double TotalMilliseconds { get; set; }

        public long? ContentLength { get; set; }
    }
}
