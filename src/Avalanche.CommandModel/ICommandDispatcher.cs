
namespace Avalanche.CommandModel
{
    public interface ICommandDispatcher : IDisposable
    {
        void Add(ICommand metric);

        void End();
    }
}