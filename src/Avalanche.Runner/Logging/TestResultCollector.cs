namespace Avalanche.Runner.Logging
{
    public class TestResultCollector : ITestResultCollector
    {
        private readonly Queue<LogEvent> _queue = new();

        private readonly ManualResetEvent _waitHandle = new(false);
        private bool _isRunning;

        public TestResultCollector(TestResultsCollection collection)
        {
            StartCollector(collection);
        }

        public void Add(LogEvent metric)
        {
            _queue.Enqueue(metric);
            _waitHandle.Reset();
        }

        public void StartCollector(TestResultsCollection collection)
        {

            _isRunning = true;

            Task.Factory.StartNew(() =>
                {
                    while (_isRunning)
                    {
                        _waitHandle.Reset();

                        var entry = _queue.Any() ? _queue.Dequeue() : null;
                        while (entry != null)
                        {
                            collection.Add(entry);

                            if(string.IsNullOrEmpty(collection.ThreadId) && entry is IterationLogEvent ie)
                            {
                                collection.ThreadId = ie.Thread.ToString();
                            }

                            entry = _queue.Any() ? _queue.Dequeue() : null;

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

        public void End()
        {
            _isRunning = false;
            _waitHandle.Reset();
        }
    }
}
