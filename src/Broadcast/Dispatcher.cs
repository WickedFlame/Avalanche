namespace Broadcast
{
    //
    // Dispatcher uses a Queue to process data async
    // This allows the dispatcher to process big amounts of data
    //

    public class Dispatcher<T> : MessageBus, IDispatcher<T>
    {
        private readonly Queue<T> _queue = new();

        private readonly ManualResetEvent _waitHandle = new(false);
        private bool _isRunning;

        public Dispatcher()
        {
            StartDispatcher();
        }

        public void Register<Tc>(IMessageHandler<T> handler) where Tc : class, T
        {
            base.Register<Tc>(handler);
        }

        public void SendAsync<Tc>(Tc @event) where Tc : class, T
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
                            //var handler = _handlers[entry.GetType()] as IMessageHandler<T>;
                            //if(handler == null)
                            //{
                            //    continue;
                            //}

                            //handler.Handle(entry);
                            base.Send(entry);

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

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Close();
            }
        }
    }
}
