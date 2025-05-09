using Avalanche.WriteModel.Commands;

namespace Avalanche.WriteModel
{
    public class IterationElementsContainer
    {
        private readonly List<IterationCommand> _events = [];
        private DateTime _lastCheck;

        public void Add(IterationCommand @event)
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
    }
}
