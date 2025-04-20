using Avalanche.Domain;
using Avalanche.ReadModel.QueryHandlers;
using Microsoft.AspNetCore.Mvc;

namespace Avalanche.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestDataController : ControllerBase
    {
        [HttpGet]
        [Route("{scenario}/{testname}/chartdata/{testId}")]
        public IActionResult GetChartData(string scenario, string testname, string testId)
        {
            var trh = new TestRunQueryHandler();
            var lastRun = trh.Get(new ReadModel.Queries.GetTestRun { TestId = testId });

            var facade = new TestDataFacade();
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
            var trh = new TestRunQueryHandler();
            var lastRun = trh.Get(new ReadModel.Queries.GetTestRun { TestId = testId });

            if(lastRun == null)
            {
                return Ok();
            }

            var facade = new TestDataFacade();
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
                    TestSummary = data.Where(d => d.Type == "TestSummary").Select(s => new
                    {
                        s.TestId,
                        s.TestCase,
                        s.Type,
                        s.ThreadNumber,
                        s.Iterations,
                        AverageTicks = TimeSpan.FromTicks(s.AverageTicks),
                        TotalTime = TimeSpan.FromTicks(s.TotalTime),
                        s.Fastest,
                        s.Slowest,
                        s.Increase,
                        s.InitialSize,
                        s.EndSize
                    }),
                    ThreadSummary = data.Where(d => d.Type == "ThreadSummary").Select(s => new
                    {
                        s.TestId,
                        s.TestCase,
                        s.Type,
                        s.ThreadNumber,
                        s.Iterations,
                        AverageTicks = TimeSpan.FromTicks(s.AverageTicks),
                        TotalTime = TimeSpan.FromTicks(s.TotalTime),
                        s.Fastest,
                        s.Slowest,
                        s.Increase,
                        s.InitialSize,
                        s.EndSize
                    })
                }
            });
        }
    }
}
