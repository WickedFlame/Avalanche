namespace Avalanche.Runner
{
    public class RequestUrl
    {
        private readonly string[] _allowedMethods =
        [
            "get",
            "post",
            "put"
        ];

        public RequestUrl(string url)
        {
            Url = url;
            Method = "GET";

            if (_allowedMethods.Any(m => url.ToLower().StartsWith($"{m} ")))
            {
                Method = url.Substring(0, url.IndexOf(" ")).TrimEnd();
                Url = url.Substring(Method.Length).TrimStart();
            }
        }

        public string Url { get; }

        public string Method { get; }
    }
}
