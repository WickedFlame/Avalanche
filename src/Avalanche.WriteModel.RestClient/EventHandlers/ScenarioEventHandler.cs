using Avalanche.WriteModel.Events;
using Broadcast;
using Microsoft.Extensions.Logging;
using RestSharp;
using System;

namespace Avalanche.WriteModel.RestClient.EventHandlers
{
    public class ScenarioEventHandler : ApiHandler,
        IEventHandler<InitScenarioEvent>
    {
        public ScenarioEventHandler(IRestClient client, ILoggerFactory loggerFactory)
            : base(client, loggerFactory)
        {
        }

        public async void Handle(InitScenarioEvent evnt)
        {
            await PostAsync($"api/event/initscenario/{evnt.Name}", evnt);
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
