namespace Broadcast
{
    public class Dispatcher<T> : IDispatcher<T>
    {
        private readonly Dictionary<Type, IDispatcherHandler> _handlers = [];

        private readonly Queue<T> _queue = new();

        private readonly ManualResetEvent _waitHandle = new(false);
        private bool _isRunning;

        public Dispatcher()
        {
            StartDispatcher();
        }

        public void Register<Tc>(IDispatcherHandler<T> handler) where Tc : class, T
        {
            _handlers[typeof(Tc)] = handler;
        }

        public void Send(T @event)
        {
            _queue.Enqueue(@event);
            _waitHandle.Reset();
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
                            var handler = _handlers[entry.GetType()] as IDispatcherHandler<T>;
                            if(handler == null)
                            {
                                continue;
                            }

                            handler.Handle(entry);

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
            }
        }
    }
}
