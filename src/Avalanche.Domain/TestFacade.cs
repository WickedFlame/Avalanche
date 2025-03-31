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
        private readonly IEventStore _store;

        public TestFacade(IEventStore store)
        {
            _store = store;
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

                    var loadtest = new LoadTest(data.TestId, _store);
                    data.StartTime = DateTime.Now;

                    using var messageBus = new EventBus(_store);
                    messageBus.Subscribe<StartTestEvent>(new TestRunEventHandler());
                    messageBus.Subscribe<EndTestEvent>(new TestRunEventHandler());
                    messageBus.Subscribe<ThreadSummaryEvent>(new SummaryEventHandler());
                    messageBus.Subscribe<TestSummaryEvent>(new SummaryEventHandler());



                    using var dispatcher = new Dispatcher<ICommand>();
                    dispatcher.Register<StartTestCommand>(new StartTestCommandHandler(messageBus));
                    dispatcher.Register<EndTestCommand>(new EndTestCommandHandler(messageBus));
                    dispatcher.Register<TestResultCommand>(new TestResultCommandHandler(_store, messageBus));



                    dispatcher.Send(new StartTestCommand
                    {
                        TestId = data.TestId,
                        Scenario = name,
                        StartTime = data.StartTime,
                        Status = TestRunStatus.Running
                    });


                    data.Results = loadtest.Run(settings);

                    foreach (var testResult in data.Results)
                    {
                        dispatcher.Send(new TestResultCommand
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


                    dispatcher.Send(new EndTestCommand
                    {
                        TestId = data.TestId,
                        EndTime = DateTime.Now,
                        Status = TestRunStatus.Done
                    });


                    //
                    // Give the collector some time to finish the work
                    Task.Delay(10000).Wait();


                    loadtest.End();

                    //
                    // write result to file
                },
                CancellationToken.None,
                TaskCreationOptions.LongRunning,
                TaskScheduler.Default);

            return settings;
        }

        public void Stop(string testId)
        {
            using var messageBus = new EventBus(_store);
            messageBus.Subscribe<EndTestEvent>(new TestRunEventHandler());

            using var dispatcher = new Dispatcher<ICommand>();
            dispatcher.Register<EndTestCommand>(new EndTestCommandHandler(messageBus));


            dispatcher.Send(new EndTestCommand
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
