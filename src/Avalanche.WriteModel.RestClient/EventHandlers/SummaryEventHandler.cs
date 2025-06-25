using Avalanche.WriteModel.Events;
using Microsoft.Extensions.Logging;
using RestSharp;
using System;

namespace Avalanche.WriteModel.RestClient.EventHandlers
{
    public class SummaryEventHandler : ApiHandler,
        IEventHandler<ThreadSummaryEvent>,
        IEventHandler<TestSummaryEvent>
    {
        public SummaryEventHandler(IRestClient client, ILoggerFactory loggerFactory)
            : base(client, loggerFactory)
        {
        }

        public async void Handle(ThreadSummaryEvent evnt)
        {
            await PostAsync("api/event/threadsummary", evnt);
        }

        public async void Handle(TestSummaryEvent evnt)
        {
            await PostAsync("api/event/testsummary", evnt);
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
