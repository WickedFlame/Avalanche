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

        private readonly ManualResetEvent _waitHandle = new(false);
        private bool _isRunning;

        public Dispatcher()
        {
            StartDispatcher();
        }

        public void Register<Tc>(IMessageHandler<T> handler) where Tc : class, T
        {
            _handlers[typeof(Tc)] = handler;
        }

        public void StartDispatcher()
        {

            _isRunning = true;

            Task.Factory.StartNew(() =>
                {
                    while (_isRunning)
                    {
                        _waitHandle.Reset();

                        var entry = _queue.Any() ? _queue.Dequeue() : default;
                        while (entry != null)
                        {
                            Send(entry);

                            entry = _queue.Any() ? _queue.Dequeue() : default;

                            if (!_isRunning)
                            {
                                break;
                            }
                        }

                        _waitHandle.WaitOne(5000);
                    }
                },
                CancellationToken.None,
                TaskCreationOptions.LongRunning,
                TaskScheduler.Default);
        }

        public void SendAsync<Tc>(Tc @event) where Tc : class, T
        {
            _queue.Enqueue(@event);
            _waitHandle.Reset();
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
            _isRunning = false;
            _waitHandle.Reset();
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
