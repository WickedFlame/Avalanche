namespace Avalanche.WriteModel.Commands
{
    public class DeleteTestRunCommand : ICommand
    {
        public string TestId { get; set; }
    }
}
