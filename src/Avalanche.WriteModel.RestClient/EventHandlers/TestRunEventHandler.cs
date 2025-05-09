using Avalanche.WriteModel.Events;
using RestSharp;
using System;

namespace Avalanche.WriteModel.RestClient.EventHandlers
{
    public class TestRunEventHandler : ApiHandler,
        IEventHandler<StartTestEvent>,
        IEventHandler<EndTestEvent>
    {
        public TestRunEventHandler(IRestClient client)
            : base(client)
        {
        }

        public async void Handle(StartTestEvent evnt)
        {
            await PostAsync("api/event/starttest", evnt);
        }

        public async void Handle(EndTestEvent evnt)
        {
            await PostAsync("api/event/endtest", evnt);
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
