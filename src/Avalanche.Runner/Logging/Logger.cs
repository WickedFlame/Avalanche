using System;
using System.Collections.Generic;
using System.Text;

namespace Avalanche.Runner.Logging
{
    //
    // Logger is per Testrun
    // LogCollection is per thread
    //

    public class Logger
    {
        private readonly List<LogCollection> _collections = new List<LogCollection>();
        private readonly object _locker = new ();

        public DateTime StartTime { get; set; }

        public LogCollection StartNew(string name)
        {
            lock (_locker)
            {
                var collection = new LogCollection(name);
                _collections.Add(collection);

                return collection;
            }
        }

        public IEnumerable<LogCollection> GetCollections()
        {
            lock (_locker)
            {
                return _collections.ToList();
            }
        }
    }

}
