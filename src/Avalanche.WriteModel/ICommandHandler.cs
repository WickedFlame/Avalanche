using Broadcast;

namespace Avalanche.WriteModel
{
    public interface ICommandHandler<in T> : Broadcast.IEventHandler<ICommand> where T : ICommand
    {
        void Handle(T @event);
    }
}
