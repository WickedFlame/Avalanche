namespace Avalanche.CommandModel.Commands
{
    public class EndTestCommand : ICommand
    {
        public string TestId { get; set; }
        public string TestName { get; set; }
        public DateTime StartTime { get; set; }
        public string Status { get; set; }
    }
}
