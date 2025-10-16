using Avalanche.Domain;
using Microsoft.AspNetCore.Mvc;
using Broadcast;
using Avalanche.WriteModel;
using Avalanche.Authentication;
using Microsoft.AspNetCore.Authorization;

namespace Avalanche.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "ApiKeyOrDefault")] // or [Authorize(AuthenticationSchemes = "ApiKey")]
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
            var path = PathMapper.GetScenarioFile(name);

            var facade = new TestFacade(_dispatcher, _loggerFactory);
            var settings = facade.StartBackgroundScenario(name, path, new TestRunSettings { Runner = TestRunnerType.Local });

            return Ok(new
            {
                Name = name,
                State = "started",
                Tests = settings.TestCases
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
