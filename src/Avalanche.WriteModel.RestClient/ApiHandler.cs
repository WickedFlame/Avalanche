using Broadcast;
using Microsoft.Extensions.Logging;
using RestSharp;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Avalanche.WriteModel.RestClient
{
    public class ApiHandler
    {
        private readonly IRestClient _client;
        private readonly ILogger<ApiHandler> _logger;

        public ApiHandler(IRestClient client, ILoggerFactory loggerFactory)
        {
            _client = client;
            _logger = loggerFactory.CreateLogger<ApiHandler>();
        }

        public async Task PostAsync(string url, object body)
        {
            try
            {
                var request = new RestRequest(url)
                    .AddBody(body);

                var res = await _client.PostAsync(request);
            }
            catch(HttpRequestException hre)
            {
                //
                // do nothing.
                // just ensure the app continues to work
                _logger.LogError(hre, $"Error sending request to {url}");
            }
            catch(Exception e)
            {
                //
                // do nothing.
                // just ensure the app continues to work
                _logger.LogError(e, $"Error sending request to {url}");
            }
        }
    }
}
