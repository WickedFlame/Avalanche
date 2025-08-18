using Moq;
using Polaroider;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Avalanche.Runner.Tests
{
    public class RequestExecutionTests
    {
        private Mock<IRestClient> _client;
        private RequestExecution _executor;

        [SetUp]
        public void SetUp()
        {
            _client = new Mock<IRestClient>();
            _client.Setup(x => x.ExecuteAsync(It.IsAny<RestRequest>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(new RestResponse { RawBytes = Array.Empty<byte>(), StatusCode = HttpStatusCode.OK }));

            _executor = new RequestExecution(_client.Object);
        }

        [Test]
        public void RequestExecution_Execute_Get()
        {
            _executor.Execute(new RequestUrl("GET https://avalanche.com"));

            _client.Verify(x => x.ExecuteAsync(It.Is<RestRequest>(r => r.Method == Method.Get), It.IsAny<CancellationToken>()));
        }

        [Test]
        public void RequestExecution_Execute_Get_Result()
        {
            var result = _executor.Execute(new RequestUrl("GET https://avalanche.com"));

            result.MatchSnapshot();
        }

        [Test]
        public void RequestExecution_Execute_Post()
        {
            _executor.Execute(new RequestUrl("POST https://avalanche.com"));

            _client.Verify(x => x.ExecuteAsync(It.Is<RestRequest>(r => r.Method == Method.Post), It.IsAny<CancellationToken>()));
        }

        [Test]
        public void RequestExecution_Execute_Post_Result()
        {
            var result = _executor.Execute(new RequestUrl("POST https://avalanche.com"));

            result.MatchSnapshot();
        }
    }
}
