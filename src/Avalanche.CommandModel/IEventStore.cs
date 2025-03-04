using System;
using System.Collections.Generic;
using System.Text;

namespace Avalanche.CommandModel
{
    public interface IEventStore
    {
        string Add<T>(string testId, string type, DateTime time, T model) where T : IEvent;
    }
}
