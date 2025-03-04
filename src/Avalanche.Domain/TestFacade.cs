using Avalanche.CommandModel;
using Avalanche.CommandModel.CommandHandlers;
using Avalanche.CommandModel.Commands;
using Avalanche.Runner;
using Avalanche.Runner.Logging;
using Broadcast;

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

                    data.Status = TestRunStatus.Running;

                    data.Settings = settings;

                    var loadtest = new LoadTest(new Runner.Logging.TestResultsFacory(data), data.TestId, _store);
                    data.StartTime = DateTime.Now;



                    using var dispatcher = new Dispatcher<ICommand>();
                    dispatcher.Register<StartTestCommand>(new StartTestCommandHandler(_store));
                    dispatcher.Register<EndTestCommand>(new EndTestCommandHandler(_store));



                    dispatcher.Send(new StartTestCommand
                    {
                        TestId = data.TestId,
                        TestName = name,
                        StartTime = data.StartTime,
                        Status = data.Status
                    });


                    data.Results = loadtest.Run(settings);

                    data.Status = TestRunStatus.Done;


                    dispatcher.Send(new EndTestCommand
                    {
                        TestId = data.TestId,
                        TestName = name,
                        StartTime = data.StartTime,
                        Status = data.Status
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

        public TestSettings GetTestSettings(string path)
        {
            var reader = new YamlMap.YamlFileReader();
            var settings = reader.Read<TestSettings>(path);

            return settings;
        }
    }
}
