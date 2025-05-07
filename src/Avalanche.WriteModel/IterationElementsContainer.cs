using Avalanche.WriteModel.Events;

namespace Avalanche.WriteModel
{
    public class IterationElementsContainer
    {
        private readonly List<IterationLogEvent> _events = new List<IterationLogEvent>();
        private DateTime _lastCheck;

        public void Add(IterationLogEvent @event)
        {
            _events.Add(@event);
        }

        public int Count()
        {
            return _events.Count;
        }

        public double GetAverageMilliseconds()
        {
            var events = _events.Skip(Math.Max(0, _events.Count - 10)).OrderBy(e => e.Time).ToList();

            if (events.Count < 10)
            {
                return 0;
            }

            return events.Average(e => e.TotalMilliseconds);
        }

        public double GetThroughput()
        {
            var events = _events.Skip(Math.Max(0, _events.Count - 10)).OrderBy(e => e.Time).ToList();

            if (events.Count < 10)
            {
                return 0;
            }

            var time = events[events.Count - 1].Time - events[0].Time;
            return events.Count / time.TotalSeconds;
        }

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
    }
}
