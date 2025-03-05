using Broadcast;

namespace Avalanche.WriteModel
{
    public interface ICommandHandler<in T> : IMessageHandler<ICommand> where T : ICommand
    {
        void Handle(T @event);
    }
}
