using System;

namespace Avalanche.CommandModel
{
    public interface ICommandHandler
    {
        void Execute<T>(string testId, T command) where T : class, ICommand;
    }
}
