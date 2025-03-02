using System;
using System.Collections.Generic;
using System.Text;

namespace Avalanche.CommandModel
{
    public class EventStore : IEventStore
    {
        public void Add<T>(string id, string type, T model) where T : ICommand
        {

        }
    }
}
