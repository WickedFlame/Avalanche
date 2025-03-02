
namespace Avalanche.CommandModel
{
    public interface ICommandDispatcher
    {
        void Add(ICommand metric);

        void End();
    }
}