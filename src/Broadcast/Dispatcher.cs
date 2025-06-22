namespace Broadcast
{
    //
    // Dispatcher uses a Queue to process data async
    // This allows the dispatcher to process big amounts of data
    //

    public class Dispatcher<T> : IDispatcher<T>, IDisposable
    {

        private readonly Dictionary<Type, IMessageHandler> _handlers = [];
        private readonly Queue<T> _queue = new();
        private readonly TimedDispatcher _dispatcher;

        public Dispatcher()
        {
            _dispatcher = new(5000, () => DispatcherTask());
            _dispatcher.StartDispatcher();
        }

        public void Register<Tc>(IMessageHandler<T> handler) where Tc : class, T
        {
            _handlers[typeof(Tc)] = handler;
        }


        private bool DispatcherTask()
        {
            var entry = _queue.Any() ? _queue.Dequeue() : default;
            while (entry != null)
            {
                Send(entry);

                entry = _queue.Any() ? _queue.Dequeue() : default;

                if (!_dispatcher.IsRunning)
                {
                    return false;
                }
            }

            return true;
        }

        public void SendAsync<Tc>(Tc @event) where Tc : class, T
        {
            _queue.Enqueue(@event);
            _dispatcher.Continue();
        }


        public void Send<Tevent>(Tevent @event)
        {
            var handler = _handlers[@event.GetType()] as IMessageHandler<Tevent>;
            if (handler == null)
            {
                return;
            }

            handler.Handle(@event);
        }

        public void Close()
        {
            _dispatcher.IsRunning = false;
            _dispatcher.Continue();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Close();
                foreach (var handler in _handlers)
                {
                    handler.Value?.Dispose();
                }
            }
        }
    }
}
