using Avalanche.WriteModel.Events;
using Microsoft.Extensions.Logging;
using RestSharp;
using System;

namespace Avalanche.WriteModel.RestClient.EventHandlers
{
    public class RampupEventHandler :
        ApiHandler,
        IEventHandler<RampupEvent>,
        IEventHandler<RampdownEvent>
    {
        public RampupEventHandler(IRestClient client, ILoggerFactory loggerFactory)
            : base(client, loggerFactory)
        {
        }

        public async void Handle(RampupEvent evnt)
        {
            if (evnt.IsWarmup)
            {
                return;
            }

            await PostAsync("api/event/rampup", evnt);
        }

        public async void Handle(RampdownEvent evnt)
        {
            if (evnt.IsWarmup)
            {
                return;
            }

            await PostAsync("api/event/rampdown", evnt);
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
                // do stuf here
            }
        }
    }
}
