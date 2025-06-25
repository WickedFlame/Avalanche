using Avalanche.Domain;
using Microsoft.AspNetCore.Mvc;
using Broadcast;
using Avalanche.WriteModel;

namespace Avalanche.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly IDispatcher<ICommand> _dispatcher;
        private readonly ILoggerFactory _loggerFactory;

        public TestController(IEventBus eventBus, ILoggerFactory loggerFactory)
        {
            _dispatcher = new CommandDispatcher(eventBus);
            _loggerFactory = loggerFactory;
        }

        [HttpPost]
        [Route("{name}/start")]
        public IActionResult Start(string name)
        {

            // LoadTest
            var path = $"./testfiles/{name}.yml";

            var facade = new TestFacade(_dispatcher, _loggerFactory);
            var settings = facade.StartBackgroundTask(name, path);

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
            var facade = new TestFacade(_dispatcher, _loggerFactory);
            facade.Stop(testId);

            return Ok(new
            {
                Name = name,
                State = "stoped"
            });
        }
    }
}
