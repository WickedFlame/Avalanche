using Avalanche.WriteModel.Commands;
using System.Collections;

namespace Avalanche.WriteModel
{
    public class IterationElementsContainer : IEnumerable<IterationCommand>
    {
        private readonly List<IterationCommand> _events = [];
        private DateTime _lastCheck;

        public void Add(IterationCommand @event)
        {
            _events.Add(@event);
        }

        public IterationCommand Last()
        {
            if(_events.Count == 0)
            {
                return null;
            }

            return _events[_events.Count - 1];
        }

        public int Count()
        {
            return _events.Count;
        }

        public bool Any()
        {
            return _events.Any();
        }

        public double GetAverageMilliseconds()
        {
            var events = _events.Count > 10 ?
                _events.Skip(Math.Max(0, _events.Count - 10)).OrderBy(e => e.Time).ToList() :
                _events.ToList();

            return events.Average(e => e.TotalMilliseconds);
        }

        public double GetThroughput()
        {
            var events = _events.Count > 50 ?
                _events.Skip(Math.Max(0, _events.Count - 50)).OrderBy(e => e.Time).ToList() :
                _events;

            if (events.Count <= 1)
            {
                return 1;
            }

            var time = events[events.Count - 1].Time - events[0].Time;
            var threads = events.GroupBy(g => g.Thread);
            return threads.Sum(t => t.Count() / time.TotalSeconds);
        }

        /// <summary>
        /// Only allow writing of events every 2 seconds
        /// </summary>
        /// <returns></returns>
        public bool IsCheckValid()
        {
            var now = DateTime.Now;
            if ((now - _lastCheck).TotalSeconds < 2)
            {
                return false;
            }

            _lastCheck = now;
            return true;
        }

        public void Merge(IterationElementsContainer entry)
        {
            foreach (var en in entry.Where(e => !e.IsWarmup))
            {
                _events.Add(en);
            }
        }

        public IEnumerator<IterationCommand> GetEnumerator()
        {
            return _events.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
