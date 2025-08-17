namespace Avalanche.Runner.Tests
{
    public class RequestUrlTests
    {
        [Test]
        public void RequestUrl_Default()
        {
            var url = new RequestUrl("https://avalanche.com");

            url.Url.Should().Be("https://avalanche.com");
            url.Method.Should().Be("GET");
        }

        [Test]
        public void RequestUrl_Get()
        {
            var url = new RequestUrl("GET https://avalanche.com");

            url.Url.Should().Be("https://avalanche.com");
            url.Method.Should().Be("GET");
        }

        [Test]
        public void RequestUrl_Post()
        {
            var url = new RequestUrl("POST https://avalanche.com");

            url.Url.Should().Be("https://avalanche.com");
            url.Method.Should().Be("POST");
        }

        [Test]
        public void RequestUrl_Put()
        {
            var url = new RequestUrl("PUT https://avalanche.com");

            url.Url.Should().Be("https://avalanche.com");
            url.Method.Should().Be("PUT");
        }
    }
}