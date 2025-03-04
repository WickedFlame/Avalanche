using Avalanche.CommandModel;
using Avalanche.CommandModel.CommandHandlers;
using Avalanche.CommandModel.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Avalanche.Runner.Logging
{
    public class CommandDispatcher : ICommandDispatcher
    {
        private readonly Queue<ICommand> _queue = new();

        private readonly ManualResetEvent _waitHandle = new(false);
        private bool _isRunning;
        private readonly Dictionary<Type, ICommandHandler> _commandHandlers;

        public CommandDispatcher(TestResultsCollection collection, IEventStore store)
        {
            _commandHandlers = new Dictionary<Type, ICommandHandler>
            {
                { typeof(StartupThreadCommand), new StartupThreadCommandHandler(store, collection) },
                { typeof(EndThreadCommand), new EndThreadCommandHandler(store, collection) },
                { typeof(IterationCommand), new IterationCommandHandler(store, collection) }
            };

            StartDispatcher();
        }

        public void Add(ICommand metric)
        {
            _queue.Enqueue(metric);
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

                        var entry = _queue.Any() ? _queue.Dequeue() : null;
                        while (entry != null)
                        {
                            _commandHandlers[entry.GetType()].Execute(entry.TestId, entry);

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

        public void Dispose()
        {
            // Dispose of unmanaged resources.
            Dispose(true);
            // Suppress finalization.
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                End();
            }
        }
    }
}
