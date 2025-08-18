using RestSharp;

namespace Avalanche.Runner
{
    public class RequestExecution
    {
        private readonly IRestClient _client;

        public RequestExecution(IRestClient client)
        {
            _client = client;
        }

        public RequestResponse Execute(RequestUrl url)
        {
            var request = new RestRequest(url.Url);

            switch (url.Method)
            {
                case "POST":
                    var res = _client.PostAsync(request).GetAwaiter().GetResult();
                    return new RequestResponse
                    {
                        StatusCode = res.StatusCode,
                        IsSuccessful = res.IsSuccessful,
                        ErrorMessage = res.ErrorMessage ?? res.ErrorException?.Message,
                        ContentLength = (long)res.RawBytes.Length
                    };

                case "GET":
                    
                    var result = _client.GetAsync(request).GetAwaiter().GetResult();

                    return new RequestResponse
                    {
                        StatusCode = result.StatusCode,
                        IsSuccessful = result.IsSuccessful,
                        ErrorMessage = result.ErrorMessage ?? result.ErrorException?.Message,
                        ContentLength = (long)result.RawBytes.Length
                    };
            }

            return new RequestResponse
            {
                IsSuccessful = false
            };
        }

        public RequestResponse Execute(RequestUrl url, string body)
        {
            var request = new RestRequest(url.Url);

            switch (url.Method)
            {
                case "POST":
                    request.AddBody(body);
                    var res = _client.PostAsync(request).GetAwaiter().GetResult();
                    return new RequestResponse
                    {
                        StatusCode = res.StatusCode,
                        IsSuccessful = res.IsSuccessful,
                        ErrorMessage = res.ErrorMessage ?? res.ErrorException?.Message,
                        ContentLength = (long)res.RawBytes.Length
                    };
            }

            return new RequestResponse
            {
                IsSuccessful = false
            };
        }
    }
}
