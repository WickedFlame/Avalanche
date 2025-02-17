using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Avalanche.Domain;

namespace Avalanche.Controllers
{
    [Route("api/test")]
    [ApiController]
    public class LogsController : ControllerBase
    {
        [HttpGet]
        [Route("{name}/logs")]
        public IActionResult Get(string name)
        {
            return Ok(new
            {
                Name = name,
                Logs = TestResultsCollection.Instance.GetResults(name)
            });
        }
    }
}
