using Microsoft.AspNetCore.Mvc;
using Avalanche.Domain;
using Avalanche.Models;
using System.Xml.Linq;
using Avalanche.Runner.Logging;
using Avalanche.Domain.Models;
using Avalanche.Runner;
using Avalanche.ReadModel.QueryHandlers;

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
            //TODO: diese methode wird nicht gebraucht???



            var trh = new TestRunQueryHandler();
            var lastRun = trh.Get(new ReadModel.Queries.GetLastTestQuery { Scenario = testname });




            var results = Domain.TestResultsCollection.Instance.GetResults(scenario.ToLower());
            if (results == null || results.Status != TestRunStatus.Done)
            {
                return Ok(new
                {
                    Scenario = scenario,
                    Testname = testname,
                    Status = lastRun?.Status ?? TestRunStatus.New.Name,
                });
            }

            return Ok(new
            {
                Scenario = scenario,
                Testname = testname,
                Status = lastRun.Status,
                Result = results.Results.FirstOrDefault(r => r.TestCase == testname)
            });
        }

        [HttpGet]
        [Route("{scenario}/{testname}/chartdata/{testId}")]
        public IActionResult GetChartData(string scenario, string testname, string testId)
        {
            var trh = new TestRunQueryHandler();
            var lastRun = trh.Get(new ReadModel.Queries.GetTestRun { TestId = testId });

            var results = Domain.TestResultsCollection.Instance.GetResults(scenario.ToLower());
            if(results == null)
            {
                return Ok();
            }

            var facade = new TestDataFacade();
            var data = facade.GetChartData(lastRun.TestId);

            return Ok(new
            {
                Scenario = scenario,
                Testname = testname,
                Status = lastRun?.Status ?? "Open",
                ChartData = data
            });
        }

        [HttpGet]
        [Route("{scenario}/{testname}/rampupdata/{testId}")]
        public IActionResult GetRampupData(string scenario, string testname, string testId)
        {
            var trh = new TestRunQueryHandler();
            var lastRun = trh.Get(new ReadModel.Queries.GetTestRun { TestId = testId });

            if(lastRun == null)
            {
                return Ok();
            }

            var facade = new TestDataFacade();
            var data = facade.GetRampupData(lastRun.TestId);

            return Ok(new
            {
                Scenario = scenario,
                Testname = testname,
                Status = lastRun?.Status ?? "New",
                Data = data
            });
        }

        [HttpGet]
        [Route("{scenario}/{testname}/summary/{testId}")]
        public IActionResult GetSummary(string scenario, string testname, string testId)
        {
            var trh = new TestRunQueryHandler();
            var lastRun = trh.Get(new ReadModel.Queries.GetTestRun { TestId = testId });

            if (lastRun == null)
            {
                return Ok();
            }

            var facade = new TestDataFacade();
            var data = facade.GetSummary(testId);

            return Ok(new
            {
                Scenario = scenario,
                Testname = testname,
                TestId = testId,
                Status = lastRun?.Status ?? "New",
                Data = new
                {
                    TestSummary = data.Where(d => d.Type == "TestSummary"),
                    ThreadSummary = data.Where(d => d.Type == "ThreadSummary")
                }
            });
        }
    }
}
