using System.Net;

namespace Avalanche.Runner
{
    public class RequestResponse
    {
        public bool IsSuccessful { get; set; }

        public string ErrorMessage { get; set; }

        public HttpStatusCode StatusCode { get; set; }

        public long ContentLength { get; set; }
    }
}
