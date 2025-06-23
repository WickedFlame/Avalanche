using Avalanche.Runner;
using Avalanche.WriteModel;
using Avalanche.WriteModel.CommandHandlers;
using Avalanche.WriteModel.Commands;
using Broadcast;
using MeasureMap;
using Task = System.Threading.Tasks.Task;

namespace Avalanche.Domain
{
    public class TestFacade
    {
        private readonly IDispatcher<ICommand> _dispatcher;

        public TestFacade(IDispatcher<ICommand> dispatcher)
        {
            _dispatcher = dispatcher;
        }

        public TestSettings StartBackgroundTask(string name, string path)
        {
            var tsr = new TestSettingsReader();
            var settings = tsr.GetTestSettings(path);

            Task.Factory.StartNew(() =>
                {
                    Run(name, settings);
                },
                CancellationToken.None,
                TaskCreationOptions.LongRunning,
                TaskScheduler.Default);

            return settings;
        }


        public TestSettings Start(string name, string path)
        {
            var tsr = new TestSettingsReader();
            var settings = tsr.GetTestSettings(path);

            Run(name, settings);

            return settings;
        }

        private void Run(string name, TestSettings settings)
        {
            var data = new TestRunData
            {
                TestId = Guid.NewGuid().ToString(),
                Name = name,
                Settings = settings
            };

            var loadtest = new LoadTest(data.TestId, _dispatcher);
            data.StartTime = DateTime.Now;


            _dispatcher.Send(new StartTestCommand
            {
                TestId = data.TestId,
                Scenario = name,
                StartTime = data.StartTime
            });


            data.Results = loadtest.Run(settings);

            foreach (var testResult in data.Results)
            {
                _dispatcher.SendAsync(new TestResultCommand
                {
                    TestId = data.TestId,
                    TestCase = testResult.TestCase,
                    ThreadNumber = testResult.ThreadNumber,
                    Iterations = testResult.Iterations.Count(),
                    AverageMilliseconds = (int)testResult.AverageMilliseconds,
                    TotalMilliseconds = testResult.Duration.TotalMilliseconds,
                    Throughput = testResult.Throughput(),
                    Slowest = testResult.Slowest.Duration.TotalMilliseconds,
                    Fastest = testResult.Fastest.Duration.TotalMilliseconds,
                    Summary = testResult.Select(r => new ThreadSummary
                    {
                        ThreadNumber = r.ThreadNumber,
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
