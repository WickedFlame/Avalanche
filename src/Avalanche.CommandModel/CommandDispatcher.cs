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

        public CommandDispatcher(string testId, TestResultsCollection collection, IEventStore store)
        {
            _commandHandlers = new Dictionary<Type, ICommandHandler>
            {
                { typeof(StartupCommand), new StartupCommandHandler(store, collection) },
                { typeof(EndCommand), new EndCommandHandler(store, collection) },
                { typeof(IterationCommand), new IterationCommandHandler(store, collection) }
            };

            StartDispatcher(testId, collection);
        }

        public void Add(ICommand metric)
        {
            _queue.Enqueue(metric);
            _waitHandle.Reset();
        }

        public void StartDispatcher(string testId, TestResultsCollection collection)
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
                            if(string.IsNullOrEmpty(collection.ThreadId) && entry is IterationCommand ie)
                            {
                                collection.ThreadId = ie.Thread.ToString();
                                collection.IsWarmup = ie.IsWarmup;
                            }

                            _commandHandlers[entry.GetType()].Execute(testId, entry);

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
