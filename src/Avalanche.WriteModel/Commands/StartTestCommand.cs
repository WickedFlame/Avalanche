namespace Avalanche.WriteModel.Commands
{
    public class StartTestCommand : ICommand
    {
        public string TestId { get; set; }

        public string TestName { get; set; }

        public DateTime StartTime { get; set; }

        public string Status { get; set; }
    }
}
