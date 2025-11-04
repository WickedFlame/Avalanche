using MeasureMap;
using Microsoft.Extensions.Logging;
using RestSharp;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Avalanche.Runner.Handlers
{
    /// <summary>
    /// Handler to add OAuth authentication to the client
    /// </summary>
    public class OAuthAuthenticationHandler : IExecutionHandler
    {
        private readonly ILogger<OAuthAuthenticationHandler> _logger;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        public OAuthAuthenticationHandler(ILoggerFactory logger)
        {
            _logger = logger.CreateLogger<OAuthAuthenticationHandler>();
        }

        /// <summary>
        /// Authenticate and add the token to the client
        /// </summary>
        /// <param name="context"></param>
        /// <param name="settings"></param>
        public void Execute(MeasureMap.ExecutionContext context, Scenario settings)
        {
            if(settings.Authorization == null || (settings.Authorization.GrantType.ToLower() != "password" && settings.Authorization.GrantType.ToLower() != "client_credentials"))
            {
                return;
            }

            try
            {
                var client = context.Get<IRestClient>("httpclient");

                // Token endpoint
                var request = new RestRequest(settings.Authorization.Authority, Method.Post);
                request.AddHeader("Content-Type", "text/plain");

                // Add form parameters
                request.AddParameter("grant_type", settings.Authorization.GrantType.ToLower());
                request.AddParameter("client_id", settings.Authorization.ClientId);
                request.AddParameter("client_secret", settings.Authorization.ClientSecret);
                request.AddParameter("scope", settings.Authorization.Scope);

                if (settings.Authorization.GrantType.ToLower() == "password")
                {
                    request.AddParameter("username", settings.Authorization.Username);
                    request.AddParameter("password", settings.Authorization.Password);
                }

                // Execute the request
                var response = client.Execute(request);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var jwt = JsonSerializer.Deserialize<Jwt>(response.Content);
                    client.AddDefaultHeader("Authorization", $"Bearer {jwt.AccessToken}");
                }
                else
                {
                    _logger.LogError("Authentication Request failed with statuscode {StatusCode} for {Authority}", response.StatusCode, settings.Authorization.Authority);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while authenticating {Client} for {Authority}", settings.Authorization.ClientId, settings.Authorization.Authority);
            }
        }

        public class Jwt
        {
            [JsonPropertyName("access_token")]
            public string AccessToken { get; set; }
        }
    }
}
