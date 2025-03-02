using System;

namespace Avalanche.CommandModel
{
    public interface ICommandHandler
    {
        void Execute<T>(string id, T command) where T : class, ICommand;
    }
}
