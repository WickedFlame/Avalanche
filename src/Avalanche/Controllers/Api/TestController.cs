using Avalanche.Domain;
using Microsoft.AspNetCore.Mvc;
using Broadcast;
using Avalanche.WriteModel.CommandHandlers;
using Avalanche.WriteModel.Commands;
using Avalanche.WriteModel;

namespace Avalanche.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly IDispatcher<ICommand> _dispatcher;

        public TestController(IDispatcher<ICommand> dispatcher)
        {
            _dispatcher = dispatcher;
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

            var facade = new TestFacade(_dispatcher);
            var settings = facade.Start(name, path);

            return Ok(new
            {
                Name = name,
                State = "started",
                Tests = settings.Tests
            });
        }

        [HttpPost]
        [Route("{name}/stop/{testId}")]
        public IActionResult Stop(string name, string testId)
        {
            var facade = new TestFacade(_dispatcher);
            facade.Stop(testId);

            return Ok(new
            {
                Name = name,
                State = "stoped"
            });
        }
    }
}
