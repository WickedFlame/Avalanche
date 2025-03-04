using Broadcast;
using System;
using System.Collections.Generic;
using System.Text;

namespace Avalanche.CommandModel
{
    public class EventDispatcher : Dispatcher<IEvent>, IEventDispatcher
    {
    }
}
