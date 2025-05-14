using Avalanche.WriteModel.Events;
using RestSharp;
using System;

namespace Avalanche.WriteModel.RestClient.EventHandlers
{
    public class IterationEventHandler : ApiHandler,
        IEventHandler<IterationLogEvent>,
        IEventHandler<IterationErrorEvent>
    {
        public IterationEventHandler(IRestClient client)
            : base(client)
        {
        }

        public async void Handle(IterationLogEvent evnt)
        {
            await PostAsync("api/event/iteration", evnt);
        }

        public async void Handle(IterationErrorEvent evnt)
        {
            await PostAsync("api/event/iteration/error", evnt);
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
            }
        }
    }
}
