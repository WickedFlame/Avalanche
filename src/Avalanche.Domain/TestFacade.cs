using Avalanche.Runner;
using Avalanche.WriteModel;
using Avalanche.WriteModel.CommandHandlers;
using Avalanche.WriteModel.Commands;
using Avalanche.WriteModel.EventHandlers;
using Avalanche.WriteModel.Events;
using Broadcast;
using Task = System.Threading.Tasks.Task;

namespace Avalanche.Domain
{
    public class TestFacade
    {
        private readonly Dispatcher<ICommand> _dispatcher;

        public TestFacade(IEventStore store)
        {
            var eventBus = new EventBus(store);
            eventBus.Subscribe<StartTestEvent>(new TestRunEventHandler());
            eventBus.Subscribe<EndTestEvent>(new TestRunEventHandler());
            eventBus.Subscribe<ThreadSummaryEvent>(new SummaryEventHandler());
            eventBus.Subscribe<TestSummaryEvent>(new SummaryEventHandler());

            eventBus.Subscribe<RampupEvent>(new RampupEventHandler());
            eventBus.Subscribe<RampdownEvent>(new RampupEventHandler());
            eventBus.Subscribe<IterationLogEvent>(new IterationEventHandler());

            _dispatcher = new Dispatcher<ICommand>();
            _dispatcher.Register<StartTestCommand>(new StartTestCommandHandler(eventBus));
            _dispatcher.Register<EndTestCommand>(new EndTestCommandHandler(eventBus));
            _dispatcher.Register<TestResultCommand>(new TestResultCommandHandler(store, eventBus));

            _dispatcher.Register<StartupThreadCommand>(new StartupThreadCommandHandler(eventBus));
            _dispatcher.Register<EndThreadCommand>(new EndThreadCommandHandler(eventBus));
            _dispatcher.Register<IterationCommand>(new IterationCommandHandler(eventBus));
        }

        public TestSettings Start(string name, string path)
        {
            var settings = GetTestSettings(path);

            Task.Factory.StartNew(() =>
                {
                    var data = new TestRunData
                    {
                        TestId = Guid.NewGuid().ToString(),
                        Name = name,
                        Status = TestRunStatus.New,
                        Settings = settings
                    };

                    var loadtest = new LoadTest(data.TestId, _dispatcher);
                    data.StartTime = DateTime.Now;


                    _dispatcher.Send(new StartTestCommand
                    {
                        TestId = data.TestId,
                        Scenario = name,
                        StartTime = data.StartTime,
                        Status = TestRunStatus.Running
                    });


                    data.Results = loadtest.Run(settings);

                    foreach (var testResult in data.Results)
                    {
                        _dispatcher.Send(new TestResultCommand
                        {
                            //TODO: The id per test/result has to be set. TestId is the overall Run ID
                            TestId = data.TestId,
                            TestCase = testResult.TestCase,
                            ThreadNumber = testResult.ThreadNumber,
                            Iterations = testResult.Iterations.Count(),
                            AverageTicks = testResult.AverageTicks,
                            TotalTime = testResult.TotalTime.Ticks,
                            Fastest = testResult.Fastest.Ticks,
                            Slowest = testResult.Slowest.Ticks,
                            Increase = testResult.Increase,
                            InitialSize = testResult.InitialSize,
                            EndSize = testResult.EndSize,
                            Summary = testResult.Select(r => new ThreadSummary
                            {
                                ThreadNumber = r.ThreadNumber,
                                Iterations = r.Iterations.Count(),
                                AverageTicks = r.AverageTicks,
                                TotalTime = r.TotalTime.Ticks,
                                Fastest = r.Fastest.Ticks,
                                Slowest = r.Slowest.Ticks,
                                Increase = r.Increase,
                                InitialSize = r.InitialSize,
                                EndSize = r.EndSize,
                            })
                        });
                    }


                    _dispatcher.Send(new EndTestCommand
                    {
                        TestId = data.TestId,
                        EndTime = DateTime.Now,
                        Status = TestRunStatus.Done
                    });


                    //
                    // Give the collector some time to finish the work
                    Task.Delay(10000).Wait();
                },
                CancellationToken.None,
                TaskCreationOptions.LongRunning,
                TaskScheduler.Default);

            return settings;
        }

        public void Stop(string testId)
        {
            _dispatcher.Send(new EndTestCommand
            {
                TestId = testId,
                EndTime = DateTime.Now,
                Status = TestRunStatus.Done
            });
        }

        public TestSettings GetTestSettings(string path)
        {
            var reader = new YamlMap.YamlFileReader();
            var settings = reader.Read<TestSettings>(path);

            return settings;
        }
    }
}
