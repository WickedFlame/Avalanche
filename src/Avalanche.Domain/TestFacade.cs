using Avalanche.Runner;
using Avalanche.WriteModel;
using Avalanche.WriteModel.Commands;
using Broadcast;
using MeasureMap;
using Microsoft.Extensions.Logging;
using Task = System.Threading.Tasks.Task;

namespace Avalanche.Domain
{
    public class TestFacade
    {
        private readonly IDispatcher<ICommand> _dispatcher;
        private readonly ILoggerFactory _loggerFactory;

        public TestFacade(IDispatcher<ICommand> dispatcher, ILoggerFactory loggerFactory)
        {
            _dispatcher = dispatcher;
            _loggerFactory = loggerFactory;
        }

        public Scenario StartBackgroundScenario(string name, string path, TestRunSettings settings)
        {
            var scenario = GetScenario(path);

            Task.Factory.StartNew(() =>
                {
                    Run(name, scenario, settings);
                },
                CancellationToken.None,
                TaskCreationOptions.LongRunning,
                TaskScheduler.Default);

            return scenario;
        }


        public Scenario Start(string name, Scenario scenario, TestRunSettings settings)
        {
            Run(name, scenario, settings);

            return scenario;
        }

        public Scenario GetScenario(string path)
        {
            var tsr = new ScenarioReader(_loggerFactory);
            var scenario = tsr.GetScenario(path);

            return scenario;
        }

        private void Run(string name, Scenario scenario, TestRunSettings settings)
        {
            var data = new TestRunData
            {
                TestId = Guid.NewGuid().ToString(),
                Name = name,
                Scenario = scenario
            };

            var loadtest = new LoadTest(data.TestId, _dispatcher, _loggerFactory);
            data.StartTime = DateTime.Now;

            _dispatcher.Send(new StartTestCommand
            {
                TestId = data.TestId,
                Scenario = name,
                StartTime = data.StartTime,
                Runner = settings.Runner
            });

            data.Results = loadtest.Run(scenario);

            foreach (var testResult in data.Results)
            {
                _dispatcher.SendAsync(new TestResultCommand
                {
                    TestId = data.TestId,
                    TestCase = testResult.TestCase,
                    ThreadId = testResult.ThreadId,
                    Iterations = testResult.Iterations.Count(),
                    AverageMilliseconds = (int)testResult.AverageMilliseconds,
                    TotalMilliseconds = testResult.Duration.TotalMilliseconds,
                    Throughput = testResult.Throughput(),
                    Slowest = testResult.Slowest.Duration.TotalMilliseconds,
                    Fastest = testResult.Fastest.Duration.TotalMilliseconds,
                    Summary = testResult.Select(r => new ThreadSummary
                    {
                        ThreadId = r.ThreadId,
                        Iterations = r.Iterations.Count(),
                        AverageMilliseconds = (int)r.AverageTicks.ToMilliseconds(),
                        TotalMilliseconds = r.TotalTime.TotalMilliseconds,
                        Throughput = r.Throughput()
                    })
                });
            }

            _dispatcher.Send(new EndTestCommand
            {
                TestId = data.TestId,
                EndTime = DateTime.Now
            });


            //
            // Give the collector some time to finish the work
            Task.Delay(10000).Wait();
        }

        public void Stop(string testId)
        {
            _dispatcher.Send(new EndTestCommand
            {
                TestId = testId,
                EndTime = DateTime.Now
            });
        }

        public void Delete(string testId)
        {
            _dispatcher.Send(new DeleteTestRunCommand
            {
                TestId = testId
            });
        }
    }
}
