namespace Avalanche.WriteModel.Commands
{
    public class StartTestCommand : ICommand
    {
        public string TestId { get; set; }

        public string Scenario { get; set; }

        public DateTime StartTime { get; set; }
    }
}
