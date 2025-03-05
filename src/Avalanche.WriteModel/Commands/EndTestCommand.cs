namespace Avalanche.WriteModel.Commands
{
    public class EndTestCommand : ICommand
    {
        public string TestId { get; set; }

        public DateTime EndTime { get; set; }

        public string Status { get; set; }
    }
}
