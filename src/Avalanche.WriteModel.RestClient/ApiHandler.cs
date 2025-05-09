using Broadcast;
using RestSharp;
using System.Threading.Tasks;

namespace Avalanche.WriteModel.RestClient
{
    public class ApiHandler
    {
        private readonly IRestClient _client;

        public ApiHandler(IRestClient client)
        {
            _client = client;
        }

        public async Task PostAsync(string url, IEvent evnt)
        {
            var request = new RestRequest(url)
                .AddBody(evnt);

            var res = await _client.PostAsync(request);
        }
    }
}
