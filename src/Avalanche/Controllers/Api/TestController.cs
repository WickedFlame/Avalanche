using Avalanche.CommandModel;
using Avalanche.Domain;
using Microsoft.AspNetCore.Mvc;

namespace Avalanche.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly IEventStore _store;

        public TestController(IEventStore store)
        {
            _store = store;
        }

        [HttpGet]
        [Route("{name}")]
        public IActionResult Get(string name)
        {
            return Ok(new
            {
                Name = name,
                StartTime = DateTime.Now.ToString("o"),
                State = "running",
                Logs = new[]
                {
                    "Started",
                    "Line 1",
                    "Line 2"
                }
            });
        }

        [HttpPost]
        [Route("{name}/start")]
        public IActionResult Start(string name)
        {

            // LoadTest
            var path = $"./testfiles/{name}.yml";

            var facade = new TestFacade(_store);
            var settings = facade.Start(name, path);

            return Ok(new
            {
                Name = name,
                State = "started",
                Tests = settings.Tests
            });
        }

        [HttpPost]
        [Route("{name}/stop")]
        public IActionResult Stop(string name)
        {
            return Ok(new
            {
                Name = name,
                State = "stoped"
            });
        }
    }
}
