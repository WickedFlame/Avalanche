using Avalanche.DataSource;
using Avalanche.Domain;
using Microsoft.AspNetCore.Mvc;

namespace Avalanche.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestsController : ControllerBase
    {
        private readonly IProjectionConnectionBuilder _builder;

        public TestsController(IProjectionConnectionBuilder builder)
        {
            _builder = builder;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var facade = new TestsFacade(_builder);
            return Ok(facade.GetScenarios());
        }
    }
}
