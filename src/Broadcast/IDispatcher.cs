using System;

namespace Broadcast
{
    public interface IDispatcher : IDisposable
    {
    }

    public interface IDispatcher<T> : IDispatcher
    {
        void Register<Tc>(IDispatcherHandler<T> handler) where Tc : class, T;

        void Send(T @event);

        void Close();
    }
}
