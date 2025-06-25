using Avalanche.WriteModel.Events;
using Broadcast;
using Microsoft.Extensions.Logging;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Avalanche.WriteModel.RestClient.EventHandlers
{
    public class IterationEventHandler : ApiHandler,
        IEventHandler<IterationLogEvent>,
        IEventHandler<IterationErrorEvent>
    {
        private readonly object _lock = new();

        private readonly TimedDispatcher _dispatcher;
        private readonly List<IterationLogEvent> _events = [];

        public IterationEventHandler(IRestClient client, ILoggerFactory loggerFactory)
            : base(client, loggerFactory)
        {
            _dispatcher = new(5000, () => DispatcherTask());
            _dispatcher.StartDispatcher();
        }

        public void Handle(IterationLogEvent evnt)
        {
            lock(_lock)
            {
                _events.Add(evnt);
            }
        }

        public async void Handle(IterationErrorEvent evnt)
        {
            await PostAsync("api/event/iteration/error", evnt);
        }


        private bool DispatcherTask()
        {
            DispatcherAsync().Wait();
            return true;
        }

        private async Task<bool> DispatcherAsync()
        {
            if (_events.Count == 0)
            {
                return true;
            }

            var events = _events.ToList();

            lock (_lock)
            {
                _events.Clear();
            }

            await PostAsync("api/event/iterations", events);

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
}
