using System;
using System.Collections.Generic;
using System.Text;

namespace Avalanche.CommandModel
{
    public interface IEventStore
    {
        void Add<T>(string id, string type, T model) where T : ICommand;
    }
}
