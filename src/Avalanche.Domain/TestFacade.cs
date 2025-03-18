using Avalanche.WriteModel;
using Avalanche.WriteModel.CommandHandlers;
using Avalanche.WriteModel.Commands;
using Avalanche.WriteModel.EventHandlers;
using Avalanche.WriteModel.Events;
using Avalanche.Runner;
using Avalanche.Runner.Logging;
using Broadcast;
using MeasureMap;
using Task = System.Threading.Tasks.Task;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
                    //TODO: nicht via singleton lösen
                    var data = TestResultsCollection.Instance.StartNew(Guid.NewGuid().ToString(), name);

                    data.Settings = settings;

                    var loadtest = new LoadTest(new Runner.Logging.TestResultsFacory(data), data.TestId, _store);
                    data.StartTime = DateTime.Now;

                    using var messageBus = new MessageBus();
                    messageBus.Register<StartTestEvent>(new StartTestEventHandler());
                    messageBus.Register<EndTestEvent>(new EndTestEventHandler());
                    messageBus.Register<ThreadSummaryEvent>(new SummaryEventHandler());
                    messageBus.Register<TestSummaryEvent>(new SummaryEventHandler());



                    using var dispatcher = new Dispatcher<ICommand>();
                    dispatcher.Register<StartTestCommand>(new StartTestCommandHandler(_store, messageBus));
                    dispatcher.Register<EndTestCommand>(new EndTestCommandHandler(_store, messageBus));
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
                            TestId = data.TestId,
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
            using var messageBus = new MessageBus();
            messageBus.Register<EndTestEvent>(new EndTestEventHandler());

            using var dispatcher = new Dispatcher<ICommand>();
            dispatcher.Register<EndTestCommand>(new EndTestCommandHandler(_store, messageBus));


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
