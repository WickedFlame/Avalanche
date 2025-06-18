using Avalanche.Domain;
using Avalanche.ReadModel.QueryHandlers;
using Microsoft.AspNetCore.Mvc;

namespace Avalanche.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestDataController : ControllerBase
    {
        private readonly TestRunQueryHandler _queryHandler;

        public TestDataController(TestRunQueryHandler queryHandler)
        {
            _queryHandler = queryHandler;
        }

        [HttpGet]
        [Route("{scenario}/{testname}/chartdata/{testId}")]
        public IActionResult GetChartData(string scenario, string testname, string testId)
        {
            var lastRun = _queryHandler.Get(new ReadModel.Queries.GetTestRun { TestId = testId });

            var facade = new TestDataFacade(_queryHandler);
            var data = facade.GetChartData(testId, testname);

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
            var lastRun = _queryHandler.Get(new ReadModel.Queries.GetTestRun { TestId = testId });

            if(lastRun == null)
            {
                return Ok();
            }

            var facade = new TestDataFacade(_queryHandler);
            var data = facade.GetRampupData(lastRun.TestId, testname);

            return Ok(new
            {
                Scenario = scenario,
                Testname = testname,
                Status = lastRun?.Status ?? "New",
                Data = data
            });
        }

        [HttpGet]
        [Route("{scenario}/summary/{testId}")]
        public IActionResult GetSummary(string scenario, string testId)
        {
            var lastRun = _queryHandler.Get(new ReadModel.Queries.GetTestRun { TestId = testId });

            if (lastRun == null)
            {
                return Ok();
            }

            var facade = new TestDataFacade(_queryHandler);
            var data = facade.GetSummary(testId);

            return Ok(new
            {
                Scenario = scenario,
                TestId = testId,
                Status = lastRun?.Status ?? "New",
                Data = new
                {
                    TestSummary = data.Where(d => d.Type == "TestSummary").Select(s => new
                    {
                        s.TestId,
                        s.TestCase,
                        s.Type,
                        s.ThreadNumber,
                        s.Iterations,
                        AverageMilliseconds = TimeSpan.FromMilliseconds(s.AverageMilliseconds),
                        TotalTime = TimeSpan.FromMilliseconds(s.TotalMilliseconds),
                        s.Throughput,
                        s.Failed,
                        s.Slowest,
                        s.Fastest
                    })
                }
            });
        }
    }
}
