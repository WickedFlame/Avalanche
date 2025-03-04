using System;
using System.Collections.Generic;
using System.Text;

namespace Broadcast
{
    public interface IDispatcherHandler : IDisposable
    {
    }

    public interface IDispatcherHandler<T> : IDispatcherHandler
    {
        void Handle(T @event);
    }
}
