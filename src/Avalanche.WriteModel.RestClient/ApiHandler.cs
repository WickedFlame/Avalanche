using Broadcast;
using RestSharp;
using System.Net.Http;
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
            try
            {
                var request = new RestRequest(url)
                    .AddBody(evnt);

                var res = await _client.PostAsync(request);
            }
            catch(HttpRequestException hre)
            {
                //
                // do nothing.
                // just ensure the app continues to work
            }
            catch
            {
                //
                // do nothing.
                // just ensure the app continues to work
            }
        }
    }
}
