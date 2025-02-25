using Microsoft.AspNetCore.Mvc;
using Avalanche.Domain;
using Avalanche.Models;
using System.Xml.Linq;
using Avalanche.Runner.Logging;
using Avalanche.Domain.Models;

namespace Avalanche.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestDataController : ControllerBase
    {
        [HttpGet]
        [Route("{scenario}/{testname}/testresult")]
        public IActionResult GetTestResult(string scenario, string testname)
        {
            var results = Domain.TestResultsCollection.Instance.GetResults(scenario.ToLower());
            if (results == null || results.Status != TestRunStatus.Done)
            {
                return Ok(new
                {
                    Scenario = scenario,
                    Testname = testname,
                    Status = results?.Status?.Name ?? TestRunStatus.New.Name,
                });
            }

            return Ok(new
            {
                Scenario = scenario,
                Testname = testname,
                Status = results.Status.Name,
                Result = results.Results.FirstOrDefault(r => r.Name == testname)
            });
        }

        [HttpGet]
        [Route("{scenario}/{testname}/chartdata")]
        public IActionResult GetChartData(string scenario, string testname)
        {
            var results = Domain.TestResultsCollection.Instance.GetResults(scenario.ToLower());
            if(results == null)
            {
                return Ok();
            }

            testname = testname.ToLower();

            return Ok(new
            {
                Scenario = scenario,
                Testname = testname,
                Status = results.Status?.Name ?? "Open",
                ChartData = results.LogEntries.GetChartData()
            });
        }

        [HttpGet]
        [Route("{scenario}/{testname}/rampupdata")]
        public IActionResult GetRampupData(string scenario, string testname)
        {
            var results = Domain.TestResultsCollection.Instance.GetResults(scenario.ToLower());
            if (results == null)
            {
                return Ok();
            }

            testname = testname.ToLower();

            return Ok(new
            {
                Scenario = scenario,
                Testname = testname,
                Status = results.Status?.Name ?? "Open",
                Data = results.LogEntries.GetRampupData()
            });
        }
    }
}
