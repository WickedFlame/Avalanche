using Avalanche.WriteModel.Events;
using Broadcast;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Avalanche.WriteModel.Memory.EventHandlers
{
    public class ConsoleOutpuEventHandler :
        IEventHandler<IterationLogEvent>,
        IEventHandler<IterationErrorEvent>,
        IEventHandler<EndTestEvent>,
        IEventHandler<TestSummaryEvent>
    {
        private readonly object _lock = new();

        private readonly TimedDispatcher _dispatcher;

        private readonly Dictionary<string, Dictionary<int, IterationLogEvent>> _events = [];
        private readonly List<IterationErrorEvent> _errors = [];

        public ConsoleOutpuEventHandler()
        {
            _dispatcher = new(3000, () => DispatcherTask());
            _dispatcher.StartDispatcher();
        }

        public void Handle(IterationLogEvent @event)
        {
            if (@event.IsWarmup)
            {
                return;
            }

            lock (_lock)
            {
                if (!_events.ContainsKey(@event.TestCase))
                {
                    _events[@event.TestCase] = [];
                }

                _events[@event.TestCase][@event.Thread] = @event;
            }
        }

        public void Handle(IterationErrorEvent @event)
        {
            if (@event.IsWarmup)
            {
                return;
            }

            lock (_lock)
            {
                _errors.Add(@event);
            }
        }

        public void Handle(EndTestEvent evnt)
        {
            _dispatcher.Close();
            Console.WriteLine("");
            Console.WriteLine("SUMMARY:");
        }

        public void Handle(TestSummaryEvent @event)
        {
            _dispatcher.Close();
            var errors = _errors.Count(e => e.TestCase == @event.TestCase);
            Console.WriteLine($"  {@event.TestCase.FormatTestCaseTitle()}   Users: {@event.Users}, Throughput: {@event.Throughput}/s, Iterations: {@event.Iterations}, Errors: {errors}, Average: {(int)@event.AverageMilliseconds}ms");
        }

        private bool DispatcherTask()
        {
            lock(_lock)
            {
                if(!_events.Any() && !_errors.Any())
                {
                    return true;
                }

                Console.WriteLine("");
                foreach (var test in _events)
                {
                    var errors = _errors.Where(e => e.TestCase == test.Key);
                    Console.WriteLine($"{test.Key.FormatTestCaseTitle()}   Users: {test.Value.Count}, Throughput: {test.Value.Average(t => t.Value.Throughput)}/s, Average: {(int)test.Value.Average(t => t.Value.AverageMilliseconds)}ms, Iterations: {test.Value.Sum(t => t.Value.Iterations)}, Errors: {errors.Count()}, ContentSize: {test.Value.Average(t => t.Value.ContentLength)}");
                }
            }


            return true;
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
                // do stuf here;
                _dispatcher.Close();
            }
        }
    }

    internal static class StringExtensions
    {
        public static string FormatTestCaseTitle(this string testCase)
        {
            if(testCase.Length > 15)
            {
                return testCase.Substring(0, 15);
            }

            return testCase.PadRight(15);
        }
    }
}
