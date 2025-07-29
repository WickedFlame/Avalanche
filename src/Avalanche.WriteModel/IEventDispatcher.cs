using Broadcast;
using System;
using System.Collections.Generic;
using System.Text;

namespace Avalanche.WriteModel
{
    public interface IEventDispatcher : IDispatcher<IEvent>
    {
    }
}
