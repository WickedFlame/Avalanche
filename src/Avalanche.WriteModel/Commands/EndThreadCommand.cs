
namespace Avalanche.WriteModel.Commands
{
    public class EndThreadCommand : ICommand
    {
        public EndThreadCommand()
        {
            Time = DateTime.Now;
        }

        public string TestId { get; set; }

        public DateTime Time { get; set; }

        public string Category { get; set; }

        public string Module { get; set; }

        public string TestCase { get; set; }

        public string Message { get; set; }

        public int RunNumber { get; set; }

        public int Thread { get; set; }

        public bool IsWarmup { get; set; }
    }
}
